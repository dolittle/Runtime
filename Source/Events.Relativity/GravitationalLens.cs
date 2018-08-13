/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Execution;
using Dolittle.Serialization.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IGravitationalLens"/>
    /// </summary>
    [Singleton]
    public class GravitationalLens : IGravitationalLens, IDisposable
    {
        const int _port = 50051;
        Server _server;
        readonly ILogger _logger;
        readonly IEventHorizonsConfigurationManager _configurationManager;
        readonly ISerializer _serializer;
        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of <see cref="GravitationalLens"/>
        /// </summary>
        /// <param name="configurationManager"><see cref="IEventHorizonsConfigurationManager"/> for configuration</param>
        /// <param name="serializer"><see cref="ISerializer"/> for serializing payloads</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for dealing with <see cref="IExecutionContext"/></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public GravitationalLens(
            IEventHorizonsConfigurationManager configurationManager,
            ISerializer serializer,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _logger = logger;
            _configurationManager = configurationManager;
            _serializer = serializer;
            _executionContextManager = executionContextManager;
        }

        /// <inheritdoc/>
        public void ObserveFor(IEventHorizon eventHorizon)
        {
            try
            {
                _server = new Server
                {
                    Services = { QuantumTunnelService.BindService(new QuantumTunnelServiceImplementation(eventHorizon, _serializer, _executionContextManager, _logger)) },
                    Ports = { new ServerPort("localhost", _configurationManager.Current.Port, SslServerCredentials.Insecure) }
                };

                _server
                    .Ports
                    .ForEach(_ =>
                        _logger.Information($"Starting gRPC server on {_.Host}" + (_.Port > 0 ? $" for port {_.Port}" : string.Empty)));

                _server.Start();

                _logger.Information("Server started");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Couldn't not establish an event horizon");
            }

        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if( _server != null ) _server.ShutdownAsync().Wait();
        }
    }
}