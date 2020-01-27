// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using contracts::Dolittle.Runtime.TimeSeries.DataTypes;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Delegate that represents the callback for when a series of <see cref="DataPoint"/> has been received.
    /// </summary>
    /// <param name="dataPoint"><see cref="IEnumerable{T}"/> of <see cref="DataPoint"/>.</param>
    public delegate void DataPointsReceived(IEnumerable<DataPoint> dataPoint);
}