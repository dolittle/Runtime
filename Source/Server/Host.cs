/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Dolittle.Collections;
using Dolittle.Logging;
using Grpc.Core;
using grpc = Grpc.Core;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents an implementation of <see cref="IHost"/>
    /// </summary>
    public class Host : IHost
    {
        readonly ILogger _logger;
        grpc::Server _server;

        /// <summary>
        /// Initializes a new instance of <see cref="Host"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public Host(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Destructs the <see cref="Host"/> instance
        /// </summary>
        ~Host()
        {
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _server?.ShutdownAsync().Wait();
        }

        /// <inheritdoc/>
        public void Start(string identifier, HostConfiguration configuration, IEnumerable<ServerServiceDefinition> services)
        {
            try
            {
                _server
                    .Ports
                    .ForEach(_ =>
                        _logger.Information($"Starting {identifier} host on {_.Host}" + (_.Port > 0 ? $" for port {_.Port}" : string.Empty)));

                _server = new grpc::Server
                {
                    Ports = {
                    new grpc.ServerPort("0.0.0.0", configuration.Port, grpc::SslServerCredentials.Insecure)
                    }
                };

                services.ForEach(_server.Services.Add);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Couldn't start {identifier} host");
            }
        }
    }
}