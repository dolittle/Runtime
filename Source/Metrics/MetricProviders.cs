// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Types;
using Prometheus;

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricProviders"/>.
    /// </summary>
    public class MetricProviders : IMetricProviders
    {
        readonly IInstancesOf<ICanProvideMetrics> _providers;
        readonly IMetricFactory _metricFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricProviders"/> class.
        /// </summary>
        /// <param name="providers"><see cref="IInstancesOf{T}"/> of <see cref="ICanProvideMetrics"/>.</param>
        /// <param name="metricFactory"><see cref="IMetricFactory"/>.</param>
        public MetricProviders(
            IInstancesOf<ICanProvideMetrics> providers,
            IMetricFactory metricFactory)
        {
            _providers = providers;
            _metricFactory = metricFactory;
        }

        /// <inheritdoc/>
        public IEnumerable<Collector> Provide()
        {
            var collectors = _providers.SelectMany(_ => _.Provide(_metricFactory)).ToArray();
            return collectors;
        }
    }
}