// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Configuration;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Represents a <see cref="ICanProvideDefaultConfigurationFor{T}">default provider</see> for <see cref="TimeSeriesMap"/>.
    /// </summary>
    public class TimeSeriesMapConfigurationDefaultProvider : ICanProvideDefaultConfigurationFor<TimeSeriesMap>
    {
        /// <inheritdoc/>
        public TimeSeriesMap Provide()
        {
            return new TimeSeriesMap(new Dictionary<Source, TimeSeriesByTag>());
        }
    }
}