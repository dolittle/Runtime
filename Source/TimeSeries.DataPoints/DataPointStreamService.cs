// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.TimeSeries.DataTypes;
using Dolittle.Logging;
using Dolittle.Runtime.TimeSeries.State;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static contracts::Dolittle.Runtime.TimeSeries.DataPoints.DataPointStream;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents an implementation of <see cref="DataPointStreamBase"/>.
    /// </summary>
    public class DataPointStreamService : DataPointStreamBase
    {
        readonly IDataPointsState _dataPointsState;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointStreamService"/> class.
        /// </summary>
        /// <param name="dataPointsState"><see cref="IDataPointsState"/> for working with state.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public DataPointStreamService(
            IDataPointsState dataPointsState,
            ILogger logger)
        {
            _dataPointsState = dataPointsState;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<Empty> Open(IAsyncStreamReader<DataPoint> requestStream, ServerCallContext context)
        {
            _logger.Debug($"DataPointStream opened");
            while (await requestStream.MoveNext().ConfigureAwait(false))
            {
                _dataPointsState.Set(requestStream.Current);
            }

            _logger.Debug($"DataPointStream closed");

            return new Empty();
        }
    }
}