// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Heads.Management;
using Dolittle.Protobuf;
using Grpc.Core;
using static Dolittle.Heads.Management.Heads;

namespace Dolittle.Runtime.Heads.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="HeadsBase"/>.
    /// </summary>
    public class HeadsService : HeadsBase
    {
        readonly IConnectedHeads _connectedHeads;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadsService"/> class.
        /// </summary>
        /// <param name="connectedHeads">The <see cref="IConnectedHeads"/>.</param>
        public HeadsService(IConnectedHeads connectedHeads)
        {
            _connectedHeads = connectedHeads;
        }

        /// <inheritdoc/>
        public override Task<ConnectedHeadsResponse> GetConnectedHeads(ConnectedHeadsRequest request, ServerCallContext context)
        {
            var clients = _connectedHeads.GetAll();
            var response = new ConnectedHeadsResponse();
            response.Heads.AddRange(clients.Select(_ =>
            {
                return new ConnectedHead
                {
                    Head = new HeadInfo
                    {
                        HeadId = _.HeadId.ToProtobuf(),
                        Host = _.Host,
                        Port = _.Port,
                        Runtime = _.Runtime
                    },
                    ConnectionTime = _.ConnectionTime.ToUnixTimeMilliseconds()
                };
            }));
            return Task.FromResult(response);
        }
    }
}