// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Represents the mapping between a <see cref="Tag"/> and a <see cref="TimeSeriesId"/>.
    /// </summary>
    public class TimeSeriesByTag : ReadOnlyDictionary<Tag, TimeSeriesId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesByTag"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public TimeSeriesByTag(IDictionary<Tag, TimeSeriesId> configuration)
            : base(configuration)
        {
        }
    }
}