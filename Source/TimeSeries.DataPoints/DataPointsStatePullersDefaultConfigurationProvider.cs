// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Configuration;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents a <see cref="ICanProvideDefaultConfigurationFor{T}">default configuration provider</see>
    /// for <see cref="DataPointsStatePullersConfiguration"/>.
    /// </summary>
    public class DataPointsStatePullersDefaultConfigurationProvider : ICanProvideDefaultConfigurationFor<DataPointsStatePullersConfiguration>
    {
        /// <inheritdoc/>
        public DataPointsStatePullersConfiguration Provide()
        {
            return new DataPointsStatePullersConfiguration(Array.Empty<DataPointsStateEndPoint>());
        }
    }
}
