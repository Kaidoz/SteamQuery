﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SteamQueryNet48.Interfaces
{
    public interface IUdpClient : IDisposable
    {
        bool IsConnected { get; }

        void Close();

        void Connect(IPEndPoint remoteIpEndpoint);

        Task<int> SendAsync(byte[] datagram, CancellationToken cancellationToken);

        Task<int> SendAsync(byte[] datagram);

        Task<UdpReceiveResult> ReceiveAsync();


        Task<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken);
    }
}

