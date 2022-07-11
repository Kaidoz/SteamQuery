﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using SteamQueryNet.Interfaces;
using SteamQueryNet.Models;
using SteamQueryNet.Tests.Responses;
using SteamQueryNet.Utils;
using xRetry;
using Xunit;

namespace SteamQueryNet.Tests;

public class ServerQueryTests
{
    private const string IP_ADDRESS = "127.0.0.1";
    private const string HOST_NAME = "localhost";
    private const ushort PORT = 27015;
    private readonly IPEndPoint _localIpEndpoint = new(IPAddress.Parse("127.0.0.1"), 0);
    private byte _packetCount;

    [Theory]
    [InlineData(IP_ADDRESS)]
    [InlineData(HOST_NAME)]
    public void ShouldInitializeWithProperHost(string host)
    {
        using (var sq = new ServerQuery(new Mock<IUdpClient>().Object, It.IsAny<IPEndPoint>()))
        {
            sq.Connect(host, PORT);
        }
    }

    [Theory]
    [InlineData("127.0.0.1:27015")]
    [InlineData("127.0.0.1,27015")]
    [InlineData("localhost:27015")]
    [InlineData("localhost,27015")]
    [InlineData("steam://connect/localhost:27015")]
    [InlineData("steam://connect/127.0.0.1:27015")]
    public void ShouldInitializeWithProperHostAndPort(string ipAndHost)
    {
        using (var sq = new ServerQuery(new Mock<IUdpClient>().Object, It.IsAny<IPEndPoint>()))
        {
            sq.Connect(ipAndHost);
        }
    }

    [Theory]
    [InlineData("invalidHost:-1")]
    [InlineData("invalidHost,-1")]
    [InlineData("invalidHost:65536")]
    [InlineData("invalidHost,65536")]
    [InlineData("256.256.256.256:-1")]
    [InlineData("256.256.256.256,-1")]
    [InlineData("256.256.256.256:65536")]
    [InlineData("256.256.256.256,65536")]
    public void ShouldNotInitializeWithAnInvalidHostAndPort(string invalidHost)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            using (var sq = new ServerQuery(new Mock<IUdpClient>().Object, It.IsAny<IPEndPoint>()))
            {
                sq.Connect(invalidHost);
            }
        });
    }

    [Fact]
    public void GetServerInfo_ShouldPopulateCorrectServerInfo()
    {
        (var responsePacket, var responseObject) = ResponseHelper.GetValidResponse(ResponseHelper.ServerInfo);
        var expectedObject = (ServerInfo)responseObject;

        byte[][] requestPackets = { RequestHelpers.PrepareAS2_INFO_Request() };
        byte[][] responsePackets = { responsePacket };

        var udpClientMock = SetupReceiveResponse(responsePackets);
        SetupRequestCompare(requestPackets, udpClientMock);

        using (var sq = new ServerQuery(udpClientMock.Object, _localIpEndpoint))
        {
            Assert.Equal(JsonConvert.SerializeObject(expectedObject), JsonConvert.SerializeObject(sq.GetServerInfo()));
        }
    }

    [Fact]
    public async Task GetPlayers_ShouldPopulateCorrectPlayers()
    {
        (var playersPacket, var responseObject) = ResponseHelper.GetValidResponse(ResponseHelper.GetPlayers);
        var expectedObject = (List<Player>)responseObject;

        var challengePacket = RequestHelpers.PrepareAS2_RENEW_CHALLENGE_Request();

        // Both requests will be executed on AS2_PLAYER since thats how you refresh challenges.
        byte[][] requestPackets = { challengePacket, challengePacket };

        // First response is the Challenge renewal response and the second 
        byte[][] responsePackets = { challengePacket, playersPacket };

        var udpClientMock = SetupReceiveResponse(responsePackets);
        SetupRequestCompare(requestPackets, udpClientMock);


        using (var sq = new ServerQuery(udpClientMock.Object, _localIpEndpoint))
        {
            Assert.Equal(JsonConvert.SerializeObject(expectedObject),
                JsonConvert.SerializeObject(await sq.GetPlayersAsync()));
        }
    }

    private void SetupRequestCompare(IEnumerable<byte[]> requestPackets, Mock<IUdpClient> udpClientMock)
    {
        udpClientMock
            .Setup(x => x.SendAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .Callback<byte[], CancellationToken>((request, _) =>
            {
                Assert.True(TestValidators.CompareBytes(requestPackets.ElementAt(_packetCount), request));
                ++_packetCount;
            });
    }

    private Mock<IUdpClient> SetupReceiveResponse(IEnumerable<byte[]> udpPackets)
    {
        var udpClientMock = new Mock<IUdpClient>();
        var setupSequence = udpClientMock.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()));
        foreach (var packet in udpPackets)
            setupSequence = setupSequence.ReturnsAsync(new UdpReceiveResult(packet, _localIpEndpoint));

        return udpClientMock;
    }
}