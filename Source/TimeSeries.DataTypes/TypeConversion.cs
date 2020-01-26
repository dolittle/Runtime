// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.TimeSeries.DataTypes
{
    /// <summary>
    /// Holds converters to convert between different Protobuf representations of types.
    /// </summary>
    public static class TypeConversion
    {
        /// <summary>
        /// Convert from a <see cref="TimeSeries.DataTypes.Runtime.DataPoint"/> to <see cref="TimeSeries.DataTypes.Microservice.DataPoint"/>.
        /// </summary>
        /// <param name="dataPoint"><see cref="TimeSeries.DataTypes.Runtime.DataPoint"/> to convert from.</param>
        /// <returns>Converted <see cref="TimeSeries.DataTypes.Microservice.DataPoint"/>.</returns>
        public static TimeSeries.DataTypes.Microservice.DataPoint ToMicroservice(this TimeSeries.DataTypes.Runtime.DataPoint dataPoint)
        {
            var converted = new TimeSeries.DataTypes.Microservice.DataPoint
            {
                TimeSeries = dataPoint.TimeSeries,
                Timestamp = dataPoint.Timestamp
            };

            switch (dataPoint.MeasurementCase)
            {
                case TimeSeries.DataTypes.Runtime.DataPoint.MeasurementOneofCase.SingleValue:
                    converted.SingleValue = dataPoint.SingleValue.ToMicroservice();
                    break;
                case TimeSeries.DataTypes.Runtime.DataPoint.MeasurementOneofCase.Vector2Value:
                    converted.Vector2Value = new TimeSeries.DataTypes.Microservice.Vector2
                    {
                        X = dataPoint.Vector2Value.X.ToMicroservice(),
                        Y = dataPoint.Vector2Value.Y.ToMicroservice()
                    };
                    break;
                case TimeSeries.DataTypes.Runtime.DataPoint.MeasurementOneofCase.Vector3Value:
                    converted.Vector3Value = new TimeSeries.DataTypes.Microservice.Vector3
                    {
                        X = dataPoint.Vector3Value.X.ToMicroservice(),
                        Y = dataPoint.Vector3Value.Y.ToMicroservice(),
                        Z = dataPoint.Vector3Value.Z.ToMicroservice()
                    };
                    break;
            }

            return converted;
        }

        /// <summary>
        /// Convert from a <see cref="TimeSeries.DataTypes.Runtime.Single"/> to <see cref="TimeSeries.DataTypes.Microservice.Single"/>.
        /// </summary>
        /// <param name="measurement"><see cref="TimeSeries.DataTypes.Runtime.Single"/> to convert from.</param>
        /// <returns>Converted <see cref="TimeSeries.DataTypes.Microservice.Single"/>.</returns>
        public static TimeSeries.DataTypes.Microservice.Single ToMicroservice(this TimeSeries.DataTypes.Runtime.Single measurement)
        {
            return new TimeSeries.DataTypes.Microservice.Single
            {
                Value = measurement.Value,
                Error = measurement.Error
            };
        }

        /// <summary>
        /// Convert from a <see cref="TimeSeries.DataTypes.Microservice.DataPoint"/> to <see cref="TimeSeries.DataTypes.Runtime.DataPoint"/>.
        /// </summary>
        /// <param name="dataPoint"><see cref="TimeSeries.DataTypes.Microservice.DataPoint"/> to convert from.</param>
        /// <returns>Converted <see cref="TimeSeries.DataTypes.Runtime.DataPoint"/>.</returns>
        public static TimeSeries.DataTypes.Runtime.DataPoint ToRuntime(this TimeSeries.DataTypes.Microservice.DataPoint dataPoint)
        {
            var converted = new TimeSeries.DataTypes.Runtime.DataPoint
            {
                TimeSeries = dataPoint.TimeSeries,
                Timestamp = dataPoint.Timestamp
            };

            switch (dataPoint.MeasurementCase)
            {
                case TimeSeries.DataTypes.Microservice.DataPoint.MeasurementOneofCase.SingleValue:
                    converted.SingleValue = dataPoint.SingleValue.ToRuntime();
                    break;
                case TimeSeries.DataTypes.Microservice.DataPoint.MeasurementOneofCase.Vector2Value:
                    converted.Vector2Value = new TimeSeries.DataTypes.Runtime.Vector2
                    {
                        X = dataPoint.Vector2Value.X.ToRuntime(),
                        Y = dataPoint.Vector2Value.Y.ToRuntime()
                    };
                    break;
                case TimeSeries.DataTypes.Microservice.DataPoint.MeasurementOneofCase.Vector3Value:
                    converted.Vector3Value = new TimeSeries.DataTypes.Runtime.Vector3
                    {
                        X = dataPoint.Vector3Value.X.ToRuntime(),
                        Y = dataPoint.Vector3Value.Y.ToRuntime(),
                        Z = dataPoint.Vector3Value.Z.ToRuntime()
                    };
                    break;
            }

            return converted;
        }

        /// <summary>
        /// Convert from a <see cref="TimeSeries.DataTypes.Microservice.Single"/> to <see cref="TimeSeries.DataTypes.Runtime.Single"/>.
        /// </summary>
        /// <param name="measurement"><see cref="TimeSeries.DataTypes.Microservice.Single"/> to convert from.</param>
        /// <returns>Converted <see cref="TimeSeries.DataTypes.Runtime.Single"/>.</returns>
        public static TimeSeries.DataTypes.Runtime.Single ToRuntime(this TimeSeries.DataTypes.Microservice.Single measurement)
        {
            return new TimeSeries.DataTypes.Runtime.Single
            {
                Value = measurement.Value,
                Error = measurement.Error
            };
        }
    }
}