/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;
using System.Timers;
using Dolittle.Collections;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Application.Grpc;
using Dolittle.Time;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents an implementation of <see cref="ClientBase"/>
    /// </summary>
    public class ClientsService : Grpc.Server.Clients.ClientsBase
    {
        readonly IClients _clients;
        readonly ISystemClock _systemClock;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ILogger"/>
        /// </summary>
        /// <param name="clients"><see cref="IClients"/> for working with connected clients</param>
        /// <param name="systemClock"><see cref="ISystemClock"/> for time</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public ClientsService(
            IClients clients,
            ISystemClock systemClock,
            ILogger logger)
        {
            _clients = clients;
            _systemClock = systemClock;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void ClientDisconnected(Client client)
        {
            _clients.Disconnect(client.ClientId);
        }

        /// <inheritdoc/>
        public override Task Connect(ClientInfo request, IServerStreamWriter<Empty> responseStream, ServerCallContext context)
        {
            var clientId = request.ClientId.To<ClientId>();
            try
            {
                _logger.Information($"Client connected '{clientId}'");
                if (request.ServicesByName.Count == 0) _logger.Information("Not providing any client services");
                else request.ServicesByName.ForEach(_ => _logger.Information($"Providing service {_}"));

                var connectionTime = _systemClock.GetCurrentTime();
                var client = new Client(
                    clientId,
                    request.Host,
                    request.Port,
                    request.Runtime,
                    request.ServicesByName,
                    connectionTime
                );

                _clients.Connect(client);

                var timer = new Timer(1000);
                timer.Enabled = true;
                timer.Elapsed += (s,e) => responseStream.WriteAsync(new Empty());

                context.CancellationToken.ThrowIfCancellationRequested();
                context.CancellationToken.WaitHandle.WaitOne();
            }
            finally
            {
                _clients.Disconnect(clientId);
            }
            return Task.CompletedTask;
        }
    }
}