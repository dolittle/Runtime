// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Heads;
using contracts::Dolittle.Runtime.Heads.Management;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Management;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Heads.Management.Heads;

namespace Dolittle.Runtime.Heads.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="HeadsBase"/>.
    /// </summary>
    public class HeadsService : HeadsBase
    {
        readonly IConnectedHeads _connectedHeads;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadsService"/> class.
        /// </summary>
        /// <param name="connectedHeads">The <see cref="IConnectedHeads"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public HeadsService(IConnectedHeads connectedHeads, ILogger logger)
        {
            _connectedHeads = connectedHeads;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task GetConnectedHeads(ConnectedHeadsRequest request, IServerStreamWriter<ConnectedHeadsResponse> responseStream, ServerCallContext context)
        {
            await _connectedHeads.All.Forward(
                responseStream,
                context,
                _ => _.Heads,
                _ => new ConnectedHead
                {
                    Head = new HeadInfo
                    {
                        HeadId = _.HeadId.ToProtobuf(),
                        Host = _.Host,
                        Runtime = _.Runtime,
                        Version = _.Version,
                        ConnectionTime = Timestamp.FromDateTimeOffset(_.ConnectionTime)
                    },
                    ConnectionTime = _.ConnectionTime.ToUnixTimeMilliseconds()
                }).ConfigureAwait(false);
        }
    }
}