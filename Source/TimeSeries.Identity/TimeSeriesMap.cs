// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Configuration;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Represents the configuration for timeseries and their relationship to source and tags.
    /// </summary>
    [Name("timeseriesmap")]
    public class TimeSeriesMap :
        ReadOnlyDictionary<Source, TimeSeriesByTag>,
        IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesMap"/> class.
        /// </summary>
        /// <param name="timeSeriesByTag">Dictionary to initialize configuration with.</param>
        public TimeSeriesMap(IDictionary<Source, TimeSeriesByTag> timeSeriesByTag)
            : base(timeSeriesByTag)
        {
        }
    }
}