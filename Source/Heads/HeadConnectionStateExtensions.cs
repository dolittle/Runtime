// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Grpc.Core;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Extension methods for dealing with connection state for clients.
    /// </summary>
    public static class HeadConnectionStateExtensions
    {
        /// <summary>
        /// The <see cref="IConnectedHeads"/>.
        /// </summary>
        internal static IConnectedHeads ConnectedHeads;

        /// <summary>
        /// Hook up a <see cref="HeadDisconnected"/> delegate that gets called when a client.
        /// on the given <see cref="ServerCallContext"/> disconnects.
        /// </summary>
        /// <param name="serverCallContext">The <see cref="ServerCallContext"/>.</param>
        /// <param name="disconnected">The <see cref="HeadDisconnected"/> callback.</param>
        public static void OnDisconnected(this ServerCallContext serverCallContext, HeadDisconnected disconnected)
        {
            var clientIdEntry = serverCallContext.RequestHeaders.SingleOrDefault(_ => _.Key.Equals("clientid", StringComparison.InvariantCultureIgnoreCase));
            if (clientIdEntry != null)
            {
                var clientId = (HeadId)Guid.Parse(clientIdEntry.Value);
                var client = ConnectedHeads.GetById(clientId);
                if (client != null)
                {
                    client.Disconnected += disconnected;
                }
            }
        }
    }
}