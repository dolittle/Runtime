// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Booting;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for data points.
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IDataPointsStatePullers _dataPointsStatePullers;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootProcedure"/> class.
        /// </summary>
        /// <param name="dataPointsStatePullers"><see cref="IDataPointsStatePullers"/> for pulling data points.</param>
        public BootProcedure(IDataPointsStatePullers dataPointsStatePullers)
        {
            _dataPointsStatePullers = dataPointsStatePullers;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            _dataPointsStatePullers.Start();
        }
    }
}
