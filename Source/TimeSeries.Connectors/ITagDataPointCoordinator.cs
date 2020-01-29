// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using grpc = contracts::Dolittle.Runtime.TimeSeries.DataPoints;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Defines a system that is capable of coordinating the work for <see cref="grpc.TagDataPoint">tag based datapoints</see>.
    /// </summary>
    public interface ITagDataPointCoordinator
    {
        /// <summary>
        /// Handle <see cref="IEnumerable{T}"/> of <see cref="grpc.TagDataPoint"/>.
        /// </summary>
        /// <param name="connectorName">Name of the connector.</param>
        /// <param name="dataPoints"><see cref="grpc.TagDataPoint">Tag data points</see> to handle.</param>
        void Handle(string connectorName, IEnumerable<grpc.TagDataPoint> dataPoints);
    }
}