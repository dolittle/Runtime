// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Metrics;
using Dolittle.TimeSeries.DataTypes.Microservice;
using Dolittle.Runtime.TimeSeries.Identity;

namespace Dolittle.Runtime.TimeSeries.State
{
    /// <summary>
    /// Represents an implementation of <see cref="IDataPointsState"/>.
    /// </summary>
    [Singleton]
    public class DataPointsState : IDataPointsState
    {
        const string ValueTrait = "value";
        const string ErrorTrait = "error";
        const string XProperty = "x";
        const string YProperty = "y";
        const string ZProperty = "z";

        readonly ConcurrentDictionary<TimeSeriesId, DataPoint> _dataPointsPerTimeSeries = new ConcurrentDictionary<TimeSeriesId, DataPoint>();
        readonly IMetricFactory _metricFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointsState"/> class.
        /// </summary>
        /// <param name="metricFactory"><see cref="IMetricFactory"/> for creating stateful metrics.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public DataPointsState(
            IMetricFactory metricFactory,
            ILogger logger)
        {
            _metricFactory = metricFactory;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<DataPoint> GetAll()
        {
            return _dataPointsPerTimeSeries.Values;
        }

        /// <inheritdoc/>
        public void Set(DataPoint dataPoint)
        {
            try
            {
                var timeSeries = dataPoint.TimeSeries.ToGuid();
                _dataPointsPerTimeSeries[timeSeries] = dataPoint;
                var gauge = _metricFactory.Gauge("datapoint", "This is a timeseries datapoint", "timeseries", "property", "trait");

                switch (dataPoint.MeasurementCase)
                {
                    case DataPoint.MeasurementOneofCase.SingleValue:
                        gauge.WithLabels(timeSeries.ToString(), string.Empty, ValueTrait).Set(dataPoint.SingleValue.Value);
                        gauge.WithLabels(timeSeries.ToString(), string.Empty, ErrorTrait).Set(dataPoint.SingleValue.Error);
                        break;

                    case DataPoint.MeasurementOneofCase.Vector2Value:
                        gauge.WithLabels(timeSeries.ToString(), XProperty, ValueTrait).Set(dataPoint.Vector2Value.X.Value);
                        gauge.WithLabels(timeSeries.ToString(), XProperty, ErrorTrait).Set(dataPoint.Vector2Value.X.Error);
                        gauge.WithLabels(timeSeries.ToString(), YProperty, ValueTrait).Set(dataPoint.Vector2Value.Y.Value);
                        gauge.WithLabels(timeSeries.ToString(), YProperty, ErrorTrait).Set(dataPoint.Vector2Value.Y.Error);
                        break;

                    case DataPoint.MeasurementOneofCase.Vector3Value:
                        gauge.WithLabels(timeSeries.ToString(), XProperty, ValueTrait).Set(dataPoint.Vector3Value.X.Value);
                        gauge.WithLabels(timeSeries.ToString(), XProperty, ErrorTrait).Set(dataPoint.Vector3Value.X.Error);
                        gauge.WithLabels(timeSeries.ToString(), YProperty, ValueTrait).Set(dataPoint.Vector3Value.Y.Value);
                        gauge.WithLabels(timeSeries.ToString(), YProperty, ErrorTrait).Set(dataPoint.Vector3Value.Y.Error);
                        gauge.WithLabels(timeSeries.ToString(), ZProperty, ValueTrait).Set(dataPoint.Vector3Value.Z.Value);
                        gauge.WithLabels(timeSeries.ToString(), ZProperty, ErrorTrait).Set(dataPoint.Vector3Value.Z.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error setting datapoint gauges");
            }
        }
    }
}