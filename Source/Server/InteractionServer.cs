/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Types;
using grpc = Grpc.Core;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents an implementation of <see cref="IInteractionServer"/>
    /// </summary>
    [Singleton]
    public class InteractionServer : IInteractionServer, IDisposable
    {
        readonly IInstancesOf<ICanBindInteractionServices> _services;
        readonly grpc::Server _server;
        readonly ILogger _logger;
        readonly Configuration _configuration;

        /// <summary>
        /// Initializes a new instance of <see cref="InteractionServer"/>
        /// </summary>
        /// <param name="services"><see cref="IInstancesOf{ICanBindInteractionServices}">Binders of interaction services</see></param>
        /// <param name="configuration"><see cref="Configuration"/> for <see cref="Configuration"/></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public InteractionServer(
                IInstancesOf<ICanBindInteractionServices> services,
                Configuration configuration,
                ILogger logger)
        {
            _services = services;
            _logger = logger;
            _configuration = configuration;
            _server = CreateServer();
        }

        /// <summary>
        /// Destructs the <see cref="InteractionServer"/>
        /// </summary>
        ~InteractionServer()
        {
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_server != null) _server.ShutdownAsync().Wait();
        }

        /// <inheritdoc/>
        public void Start()
        {
            try
            {
                _server
                    .Ports
                    .ForEach(_ =>
                        _logger.Information($"Starting interaction server on {_.Host}" + (_.Port > 0 ? $" for port {_.Port}" : string.Empty)));

                _server.Start();

                _logger.Information("Interaction server is running");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Couldn't not start interaction server");
            }
        }

        grpc::Server CreateServer()
        {
            var serviceDefinitions = _services.SelectMany(_ => _.BindServices());
            var server = new grpc::Server
            {
                Ports = {
                    new grpc.ServerPort("localhost", _configuration.Interaction.Port, grpc::SslServerCredentials.Insecure)//,
                    //new grpc.ServerPort($"unix:{_configurationManager.Current.Interaction.UnixSocket}", 0, grpc::SslServerCredentials.Insecure)
                }
            };

            serviceDefinitions.ForEach(server.Services.Add);

            return server;
        }
    }
}