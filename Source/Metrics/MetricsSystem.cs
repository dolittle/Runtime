/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Logging;
using Prometheus;

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricsSystem"/>
    /// </summary>
    public class MetricsSystem : IMetricsSystem
    {
        readonly MetricsConfiguration _configuration;
        readonly CollectorRegistry _collectorRegistry;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="MetricsSystem"/>
        /// </summary>
        /// <param name="collectorRegistry">The <see cref="CollectorRegistry"/> where metric collectors are registered</param>
        /// <param name="configuration"><see cref="MetricsConfiguration">Metrics configuration</see></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public MetricsSystem(
            CollectorRegistry collectorRegistry,
            MetricsConfiguration configuration,
            ILogger logger)
        {
            _configuration = configuration;
            _collectorRegistry = collectorRegistry;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Start()
        {
            var path = "metrics/";
            _logger.Information($"Starting metric server on port {_configuration.Port} on path '{path}'");
            var server = new MetricServer(hostname:"*", port: _configuration.Port, url:path, registry:_collectorRegistry);
            server.Start();
        }
    }
}