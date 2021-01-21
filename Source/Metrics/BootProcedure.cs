// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting;

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Represents a <see cref="ICanPerformBootProcedure">boot procedure</see> for hooking up metrics.
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IMetricProviders _metricProviders;
        readonly IMetricsSystem _metricsSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootProcedure"/> class.
        /// </summary>
        /// <param name="metricProviders"><see cref="IMetricProviders"/>.</param>
        /// <param name="metricsSystem"><see cref="IMetricsSystem"/>.</param>
        public BootProcedure(
            IMetricProviders metricProviders,
            IMetricsSystem metricsSystem)
        {
            _metricProviders = metricProviders;
            _metricsSystem = metricsSystem;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            _metricProviders.Provide();
            _metricsSystem.Start();
        }
    }
}