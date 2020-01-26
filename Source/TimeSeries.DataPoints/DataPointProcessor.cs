// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.TimeSeries.DataTypes.Runtime;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents a processor of data points.
    /// </summary>
    public class DataPointProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointProcessor"/> class.
        /// </summary>
        /// <param name="id">The <see cref="DataPointProcessorId"/> of the processor.</param>
        public DataPointProcessor(DataPointProcessorId id)
        {
            Id = id;
        }

        /// <summary>
        /// Event that gets fired when a <see cref="DataPoint"/> is received for processing
        /// </summary>
        public event DataPointsReceived Received;

        /// <summary>
        /// Gets the unique identifier for a <see cref="DataPointProcessor"/>.
        /// </summary>
        public DataPointProcessorId Id { get; }

        /// <summary>
        /// Method to call when a set of <see cref="DataPoint">DataPoints</see> are received.
        /// </summary>
        /// <param name="dataPoints"><see cref="IEnumerable{T}"/> of <see cref="DataPoint"/>.</param>
        internal void OnDataPointReceived(IEnumerable<DataPoint> dataPoints)
        {
            Received?.Invoke(dataPoints);
        }
    }
}