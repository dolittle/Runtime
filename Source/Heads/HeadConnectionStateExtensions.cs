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
            var headIdEntry = serverCallContext.RequestHeaders.SingleOrDefault(_ => _.Key.Equals($"headid{Metadata.BinaryHeaderSuffix}", StringComparison.InvariantCultureIgnoreCase));
            if (headIdEntry != null)
            {
                var headId = (HeadId)Guid.Parse(headIdEntry.Value);
                var head = ConnectedHeads.GetById(headId);
                if (head != null)
                {
                    head.Disconnected += disconnected;
                }
            }
        }
    }
}