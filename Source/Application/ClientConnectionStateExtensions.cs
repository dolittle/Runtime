/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using Grpc.Core;
using static Grpc.Core.Metadata;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Extension methods for dealing with connection state for clients
    /// </summary>
    public static class ClientConnectionStateExtensions
    {
        internal static IClients Clients;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverCallContext"></param>
        /// <param name="disconnected"></param>
        public static void OnDisconnected(this ServerCallContext serverCallContext, ClientDisconnected disconnected)
        {
            var clientIdEntry = serverCallContext.RequestHeaders.SingleOrDefault(_ => _.Key.ToLowerInvariant() == "clientid");
            if( clientIdEntry != null )
            {
                var clientId = (ClientId)Guid.Parse(clientIdEntry.Value);
                var client = Clients.GetById(clientId);
                if( client != null )
                {
                    client.Disconnected += disconnected;
                }
            }
        }
    }
}