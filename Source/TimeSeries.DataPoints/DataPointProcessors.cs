// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using contracts::Dolittle.Runtime.TimeSeries.DataTypes;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Scheduling;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents an implementation of <see cref="IDataPointProcessors"/>.
    /// </summary>
    [Singleton]
    public class DataPointProcessors : IDataPointProcessors
    {
        readonly List<DataPointProcessor> _processors = new List<DataPointProcessor>();
        readonly ILogger _logger;
        readonly IScheduler _scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointProcessors"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        /// <param name="scheduler"><see cref="IScheduler"/> for scheduling work.</param>
        public DataPointProcessors(
            ILogger logger,
            IScheduler scheduler)
        {
            _logger = logger;
            _scheduler = scheduler;
        }

        /// <inheritdoc/>
        public void Register(DataPointProcessor dataPointProcessor)
        {
            _logger.Information($"Registering '{dataPointProcessor.Id}'");
            lock (_processors) _processors.Add(dataPointProcessor);
        }

        /// <inheritdoc/>
        public void Unregister(DataPointProcessor dataPointProcessor)
        {
            _logger.Information($"Unregistering '{dataPointProcessor.Id}'");
            lock (_processors) _processors.Remove(dataPointProcessor);
        }

        /// <inheritdoc/>
        public void Process(IEnumerable<DataPoint> dataPoints)
        {
            lock (_processors)
            {
                _scheduler.PerformForEach(_processors, _ => _.OnDataPointReceived(dataPoints));
            }
        }
    }
}