// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Defines a system that is capable of pulling data point state from other Microservice runtimes.
    /// </summary>
    public interface IDataPointsStatePullers
    {
        /// <summary>
        /// Start the puller mechanism.
        /// </summary>
        void Start();
    }
}
