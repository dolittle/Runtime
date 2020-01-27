// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using contracts::Dolittle.Runtime.TimeSeries.DataTypes;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Defines a system for working with <see cref="DataPointProcessor"/>.
    /// </summary>
    public interface IDataPointProcessors
    {
        /// <summary>
        /// Register a <see cref="DataPointProcessor"/>.
        /// </summary>
        /// <param name="dataPointProcessor"><see cref="DataPointProcessor"/> to register.</param>
        void Register(DataPointProcessor dataPointProcessor);

        /// <summary>
        /// Unregister a <see cref="DataPointProcessor"/>.
        /// </summary>
        /// <param name="dataPointProcessor"><see cref="DataPointProcessor"/> to unregister.</param>
        void Unregister(DataPointProcessor dataPointProcessor);

        /// <summary>
        /// Process a series of <see cref="DataPoint"/> with any of the <see cref="DataPointProcessor"/> that
        /// is interested.
        /// </summary>
        /// <param name="dataPoints"><see cref="IEnumerable{T}"/> of <see cref="DataPoint"/> to process.</param>
        void Process(IEnumerable<DataPoint> dataPoints);
    }
}