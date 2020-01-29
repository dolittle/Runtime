// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents the configuration for a data points state endpoint.
    /// </summary>
    public class DataPointsStateEndPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointsStateEndPoint"/> class.
        /// </summary>
        /// <param name="target">The target endpoint.</param>
        /// <param name="interval">Interval to pull in milliseconds.</param>
        public DataPointsStateEndPoint(string target, double interval)
        {
            Target = target;
            Interval = interval;
        }

        /// <summary>
        /// Gets the target where the endpoint is hosted.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Gets the interval for pulling from the endpoint.
        /// </summary>
        public double Interval { get; }
    }
}
