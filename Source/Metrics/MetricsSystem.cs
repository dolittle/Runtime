// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Prometheus;

#pragma warning disable CA2213

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricsSystem"/>.
    /// </summary>
    public class MetricsSystem : IMetricsSystem, IDisposable
    {
        readonly MetricsConfiguration _configuration;
        readonly CollectorRegistry _collectorRegistry;
        readonly ILogger _logger;

        MetricServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsSystem"/> class.
        /// </summary>
        /// <param name="collectorRegistry">The <see cref="CollectorRegistry"/> where metric collectors are registered.</param>
        /// <param name="configuration"><see cref="MetricsConfiguration">Metrics configuration</see>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public MetricsSystem(
            CollectorRegistry collectorRegistry,
            MetricsConfiguration configuration,
            ILogger logger)
        {
            _configuration = configuration;
            _collectorRegistry = collectorRegistry;
            _logger = logger;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MetricsSystem"/> class.
        /// </summary>
        ~MetricsSystem()
        {
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _server?.Stop();
            _server = null;
        }

        /// <inheritdoc/>
        public void Start()
        {
            const string path = "metrics/";
            _logger.Debug("Starting metric server on port '{Port}' on path '{Path}'",
                _configuration.Port,
                path);
            _server = new MetricServer(hostname: "*", port: _configuration.Port, url: path, registry: _collectorRegistry);
            _server.Start();
        }
    }
}