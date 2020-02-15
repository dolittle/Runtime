// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Scheduling;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents an implementation of <see cref="IDataPointsStatePullers"/>.
    /// </summary>
    [Singleton]
    public class DataPointsStatePullers : IDataPointsStatePullers
    {
        readonly DataPointsStatePullersConfiguration _configuration;
        readonly ITimers _timers;
        readonly IDataPointProcessors _processors;
        readonly ILogger _logger;

        readonly List<DataPointsStatePuller> _pullers = new List<DataPointsStatePuller>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointsStatePullers"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="DataPointsStatePullersConfiguration"/>.</param>
        /// <param name="processors"><see cref="IDataPointProcessors"/> for processing.</param>
        /// <param name="timers"><see cref="ITimers"/> for scheduling.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public DataPointsStatePullers(
            DataPointsStatePullersConfiguration configuration,
            IDataPointProcessors processors,
            ITimers timers,
            ILogger logger)
        {
            _timers = timers;
            _configuration = configuration;
            _processors = processors;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Start()
        {
            _logger.Debug($"Setting up DataPointsStatePullers for {_configuration.EndPoints.Count()} endpoints");
            _configuration.EndPoints.ForEach(_ =>
            {
                _logger.Debug($"Starting a DataPointsStatePuller to pull from '{_.Target}' with interval {_.Interval}");
                _pullers.Add(new DataPointsStatePuller(_, _processors, _timers, _logger));
            });
        }
    }
}
