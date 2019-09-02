/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Application.Grpc;
using Dolittle.Time;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents an implementation of <see cref="ClientBase"/>
    /// </summary>
    public class ClientService : Grpc.Client.ClientBase
    {
        readonly IClients _clients;
        readonly ISystemClock _systemClock;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ILogger"/>
        /// </summary>
        /// <param name="clients"><see cref="IClients"/> for working with connected clients</param>
        /// <param name="systemClock"><see cref="ISystemClock"/> for time</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public ClientService(
            IClients clients,
            ISystemClock systemClock,
            ILogger logger)
        {
            _clients = clients;
            _systemClock = systemClock;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<ConnectionResult> Connect(ClientInfo request, ServerCallContext context)
        {
            _logger.Information("Client connected");
            var result = new ConnectionResult();
            try
            {
                var connectionTime = _systemClock.GetCurrentTime();
                var client = new Client(
                    new Guid(request.ClientId.Value.ToByteArray()),
                    request.Host,
                    request.Port,
                    request.Runtime,
                    connectionTime
                );
                _clients.Connect(client);
                result.Status = "Connected";
            }
            catch (Exception ex)
            {
                result.Status = $"Error on connection: {ex.Message}";
            }
            return Task.FromResult(result);

        }

        /// <inheritdoc/>
        public override Task<DisconnectedResult> Disconnect(System.Protobuf.guid clientId, ServerCallContext context)
        {
            var result = new DisconnectedResult();
            try
            { 
                var id = (ClientId) new Guid(clientId.ToByteArray());

                if (_clients.IsConnected(id))
                {
                    _logger.Information("Client disconnected");
                    _clients.Disconnect(id);
                }
            } 
            catch { }
            return Task.FromResult(result);
        }
    }
}