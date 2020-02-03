// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Exception that gets thrown when there is no <see cref="ICanIdentifyTimeSeries"/> system that
    /// can identify a <see cref="TimeSeriesId"/> from <see cref="Source"/> and <see cref="Tag"/>.
    /// </summary>
    public class NoTimeSeriesIdentifierCanIdentify : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoTimeSeriesIdentifierCanIdentify"/> class.
        /// </summary>
        /// <param name="source">The <see cref="Source"/>.</param>
        /// <param name="tag">The <see cref="Tag"/>.</param>
        public NoTimeSeriesIdentifierCanIdentify(Source source, Tag tag)
            : base($"No TimeSeries identifier can identify a tag named '{tag}' from source '{source}'")
        {
        }
    }
}