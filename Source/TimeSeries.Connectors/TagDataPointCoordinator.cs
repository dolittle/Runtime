// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Collections.Generic;
using contracts::Dolittle.Runtime.TimeSeries.DataTypes;
using Dolittle.Collections;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.TimeSeries.Identity;
using Dolittle.Runtime.TimeSeries.State;
using Google.Protobuf.WellKnownTypes;
using grpc = contracts::Dolittle.Runtime.TimeSeries.DataPoints;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Represents an implementation of <see cref="ITagDataPointCoordinator"/>.
    /// </summary>
    [Singleton]
    public class TagDataPointCoordinator : ITagDataPointCoordinator
    {
        readonly ITimeSeriesMapper _timeSeriesMapper;
        readonly IDataPointsState _dataPointsState;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagDataPointCoordinator"/> class.
        /// </summary>
        /// <param name="timeSeriesMapper"><see cref="ITimeSeriesMapper"/> for identity mapping of TimeSeries.</param>
        /// <param name="dataPointsState"><see cref="IDataPointsState"/> for working with the state.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public TagDataPointCoordinator(
            ITimeSeriesMapper timeSeriesMapper,
            IDataPointsState dataPointsState,
            ILogger logger)
        {
            _timeSeriesMapper = timeSeriesMapper;
            _logger = logger;
            _dataPointsState = dataPointsState;
        }

        /// <inheritdoc/>
        public void Handle(string connectorName, IEnumerable<grpc.TagDataPoint> dataPoints)
        {
            dataPoints.ForEach(tagDataPoint =>
            {
                if (!_timeSeriesMapper.CanIdentify(connectorName, tagDataPoint.Tag))
                {
                    _logger.Information($"Unidentified tag '{tagDataPoint.Tag}' from '{connectorName}'");
                }
                else
                {
                    _logger.Information("DataPoint received");
                    var timeSeriesId = _timeSeriesMapper.Identify(connectorName, tagDataPoint.Tag);

                    var dataPoint = new DataPoint
                    {
                        TimeSeries = timeSeriesId.ToProtobuf(),
                        Timestamp = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
                    };

                    switch (tagDataPoint.MeasurementCase)
                    {
                        case grpc.TagDataPoint.MeasurementOneofCase.SingleValue:
                            dataPoint.SingleValue = tagDataPoint.SingleValue;
                            break;
                        case grpc.TagDataPoint.MeasurementOneofCase.Vector2Value:
                            dataPoint.Vector2Value = new Vector2
                            {
                                X = tagDataPoint.Vector2Value.X,
                                Y = tagDataPoint.Vector2Value.Y
                            };
                            break;
                        case grpc.TagDataPoint.MeasurementOneofCase.Vector3Value:
                            dataPoint.Vector3Value = new Vector3
                            {
                                X = tagDataPoint.Vector3Value.X,
                                Y = tagDataPoint.Vector3Value.Y,
                                Z = tagDataPoint.Vector3Value.Z
                            };
                            break;
                    }

                    _dataPointsState.Set(dataPoint);
                }
            });
        }
    }
}