/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
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

        /// <summary>
        /// Initializes a new instance of <see cref="MetricsSystem"/>
        /// </summary>
        /// <param name="collectorRegistry">The <see cref="CollectorRegistry"/> where metric collectors are registered</param>
        /// <param name="configuration"><see cref="MetricsConfiguration">Metrics configuration</see></param>
        public MetricsSystem(
            CollectorRegistry collectorRegistry,
            MetricsConfiguration configuration)
        {
            _configuration = configuration;
            _collectorRegistry = collectorRegistry;
        }

        /// <inheritdoc/>
        public void Start()
        {
            var server = new MetricServer(hostname:"localhost", port: _configuration.Port, url:"metrics/", registry:_collectorRegistry);
            server.Start();
        }
    }
}