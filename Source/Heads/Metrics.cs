// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Specialized;
using Dolittle.Runtime.Metrics;
using Prometheus;
using IMetricFactory = Dolittle.Runtime.Metrics.IMetricFactory;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents metrics for heads.
    /// </summary>
    public class Metrics : ICanProvideMetrics
    {
        Gauge _connectedHeads;
        Counter _headConnects;
        Counter _headDisconnects;

        /// <summary>
        /// Initializes a new instance of the <see cref="Metrics"/> class.
        /// </summary>
        /// <param name="connectedHeads">The underlying <see cref="IConnectedHeads"/>.</param>
        public Metrics(IConnectedHeads connectedHeads)
        {
            connectedHeads.All.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add: for (var i = 0; i < e.NewItems?.Count; i++) _headConnects.Inc(); break;
                    case NotifyCollectionChangedAction.Remove: for (var i = 0; i < e.OldItems?.Count; i++) _headConnects.Inc(); break;
                }

                _connectedHeads.Set(connectedHeads.All.Count);
            };
        }

        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            _connectedHeads = metricFactory.Gauge("ConnectedHeads", "Number of connected heads");
            _headConnects = metricFactory.Counter("HeadConnects", "Number of connections established from heads");
            _headDisconnects = metricFactory.Counter("HeadDisconnects", "Number of connections disconnected from heads");

            return new Collector[]
            {
                _connectedHeads,
                _headConnects,
                _headDisconnects
            };
        }
    }
}