/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Prometheus;

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Defines a system for discovering metrics
    /// </summary>
    public interface ICanProvideMetrics
    {
        /// <summary>
        /// Provide a collection of <see cref="Collector">collectors</see>
        /// </summary>
        /// <param name="metricFactory"><see cref="IMetricFactory"/> for creating collectors</param>
        /// <returns>Collection of <see cref="Collector">collectors</see></returns>
        IEnumerable<Collector> Provide(IMetricFactory metricFactory);
    }
}