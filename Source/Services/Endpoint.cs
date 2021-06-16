// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Collections;
using Microsoft.Extensions.Logging;
using grpc = Grpc.Core;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IEndpoint"/>.
    /// </summary>
    public class Endpoint : IEndpoint
    {
        readonly ILogger _logger;
        grpc::Server _server;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public Endpoint(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Endpoint"/> class.
        /// </summary>
        ~Endpoint()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public void Start(EndpointVisibility type, EndpointConfiguration configuration, IEnumerable<Service> services)
        {
            try
            {
                var keepAliveTime = new grpc.ChannelOption("grpc.keepalive_time", 1000);
                var keepAliveTimeout = new grpc.ChannelOption("grpc.keepalive_timeout_ms", 500);
                var keepAliveWithoutCalls = new grpc.ChannelOption("grpc.keepalive_permit_without_calls", 1);
                _server = new grpc::Server(new[]
                {
                    keepAliveTime,
                    keepAliveTimeout,
                    keepAliveWithoutCalls
                })
                {
                    Ports =
                    {
                        new grpc.ServerPort("0.0.0.0", configuration.Port, grpc::SslServerCredentials.Insecure)
                    }
                };

                _server
                    .Ports
                    .ForEach(_ =>
                        _logger.StartingHost(type, _.Host, _.Port));

                services.ForEach(_ =>
                {
                    _logger.ExposingService(_.Descriptor.FullName);
                    _server.Services.Add(_.ServerDefinition);
                });

                _server.Start();
            }
            catch (Exception ex)
            {
                _logger.CouldNotStartHost(ex, type);
            }
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        /// <param name="disposeManagedResources">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_disposed) return;
            _server.ShutdownAsync().GetAwaiter().GetResult();
            _disposed = true;
        }
    }
}