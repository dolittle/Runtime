/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Protobuf;
using Dolittle.Runtime.Application.Grpc;
using Dolittle.Runtime.Application.Management.Grpc;
using Google.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Application.Management.Grpc.Clients;

namespace Dolittle.Runtime.Application.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ClientsBase"/>
    /// </summary>
    public class ClientsService : ClientsBase
    {
        readonly IClients _clients;

        /// <summary>
        /// Initializes a new instance of <see cref="ClientsService"/>
        /// </summary>
        /// <param name="clients">The <see cref="IClients"/></param>
        public ClientsService(IClients clients)
        {
            _clients = clients;
        }

        /// <inheritdoc/>
        public override Task<ConnectedClientsResponse> GetConnectedClients(ConnectedClientsRequest request, ServerCallContext context)
        {

            var clients = _clients.GetConnectedClients();
            var response = new ConnectedClientsResponse();
            response.Clients.AddRange(clients.Select(_ =>
            {
                return new ConnectedClient
                {
                    Client = new ClientInfo
                        {
                            ClientId = _.ClientId.ToProtobuf(),
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