// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.TimeSeries.DataTypes;
using Dolittle.Logging;
using Dolittle.Scheduling;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static contracts::Dolittle.Runtime.TimeSeries.State.DataPointsState;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents a mechanism for pulling <see cref="DataPoint">data points</see> from another runtime instance.
    /// </summary>
    public class DataPointsStatePuller
    {
        readonly DataPointsStateEndPoint _endPoint;
        readonly DataPointsStateClient _client;
        readonly IDataPointProcessors _processors;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointsStatePuller"/> class.
        /// </summary>
        /// <param name="endPoint"><see cref="DataPointsStateEndPoint"/> to pull from.</param>
        /// <param name="processors"><see cref="IDataPointProcessors"/> to involve when <see cref="DataPoint">data points</see> are pulled.</param>
        /// <param name="timers"><see cref="ITimers"/> for scheduling.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public DataPointsStatePuller(
            DataPointsStateEndPoint endPoint,
            IDataPointProcessors processors,
            ITimers timers,
            ILogger logger)
        {
            var channel = new Channel(endPoint.Target, ChannelCredentials.Insecure);
            _client = new DataPointsStateClient(channel);

            timers.Every(endPoint.Interval, async () => await Pull().ConfigureAwait(false));
            _processors = processors;
            _logger = logger;
            _endPoint = endPoint;
        }

        async Task Pull()
        {
            _logger.Information($"Pull from '{_endPoint.Target}'");
            var dataPoints = await _client.GetAllAsync(new Empty());
            _processors.Process(dataPoints.DataPoints_);
        }
    }
}
