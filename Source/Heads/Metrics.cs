// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents metrics for heads.
    /// </summary>
    public class Metrics : ICanProvideMetrics
    {
        Gauge _connectedHeads;

        /// <summary>
        /// Initializes a new instance of the <see cref="Metrics"/> class.
        /// </summary>
        /// <param name="connectedHeads">The underlying <see cref="IConnectedHeads"/>.</param>
        public Metrics(IConnectedHeads connectedHeads)
        {
            connectedHeads.All.CollectionChanged += (s, e) => _connectedHeads.Set(connectedHeads.All.Count);
        }

        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            _connectedHeads = metricFactory.Gauge("ConnectedHeads", "Number of connected heads");
            return new[]
            {
                _connectedHeads
            };
        }
    }
}