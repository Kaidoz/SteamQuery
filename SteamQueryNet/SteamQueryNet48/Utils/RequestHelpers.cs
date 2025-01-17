﻿using SteamQueryNet48.Models;
using System;
using System.Linq;
using System.Text;

namespace SteamQueryNet48.Utils
{
    internal sealed class RequestHelpers
    {
        internal static byte[] PrepareAS2_INFO_Request()
        {
            const string requestPayload = "Source Engine Query\0";
            return BuildRequest(PacketHeaders.A2S_INFO, Encoding.UTF8.GetBytes(requestPayload));
        }


        internal static byte[] PrepareAS2_INFO_Request(byte[] challenge)
        {
            const string requestPayload = "Source Engine Query\0";
            return BuildRequest(PacketHeaders.A2S_INFO,
                Encoding.UTF8.GetBytes(requestPayload).Concat(challenge).ToArray());
        }

        internal static byte[] PrepareAS2_RENEW_CHALLENGE_Request()
        {
            return BuildRequest(PacketHeaders.A2S_PLAYER, BitConverter.GetBytes(-1));
        }

        internal static byte[] PrepareAS2_GENERIC_Request(byte challengeRequestCode, int challenge)
        {
            return BuildRequest(challengeRequestCode, BitConverter.GetBytes(challenge));
        }

        private static byte[] BuildRequest(byte headerCode, byte[] extraParams = null)
        {
            /* All requests consist of 4 FF's followed by a header code to execute the request.
             * Check here: https://developer.valvesoftware.com/wiki/Server_queries#Protocol for further information about the protocol. */
            var request = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, headerCode };
            // If we have any extra payload, concatenate those into our requestHeaders and return;
            var fullRequest = extraParams != null ?
                 request.Concat(extraParams).ToArray()
                : request;
            return fullRequest;
        }
    }
}

