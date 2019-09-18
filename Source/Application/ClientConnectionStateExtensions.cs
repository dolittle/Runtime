/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using Grpc.Core;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Extension methods for dealing with connection state for clients
    /// </summary>
    public static class ClientConnectionStateExtensions
    {
        internal static IClients Clients;

        /// <summary>
        /// Hook up a <see cref="ClientDisconnected"/> delegate that gets called when a client
        /// on the given <see cref="ServerCallContext"/> disconnects
        /// </summary>
        /// <param name="serverCallContext">The <see cref="ServerCallContext"/></param>
        /// <param name="disconnected">The <see cref="ClientDisconnected"/> callback</param>
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