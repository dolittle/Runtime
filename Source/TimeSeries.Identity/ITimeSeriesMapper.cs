// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Defines a system that can translate a tag for a specific system into a timeseries identifier.
    /// </summary>
    public interface ITimeSeriesMapper
    {
        /// <summary>
        /// Check if there is a <see cref="TimeSeries"/> for a <see cref="Tag"/> in a <see cref="System"/>.
        /// </summary>
        /// <param name="source"><see cref="Source"/> the <see cref="Tag"/> belongs to.</param>
        /// <param name="tag">The actual <see cref="Tag"/>.</param>
        /// <returns>True if it exists, false if not.</returns>
        bool CanIdentify(Source source, Tag tag);

        /// <summary>
        /// Get the <see cref="TimeSeries"/> for a <see cref="Tag"/> in a <see cref="System"/>.
        /// </summary>
        /// <param name="source"><see cref="Source"/> the <see cref="Tag"/> belongs to.</param>
        /// <param name="tag">The actual <see cref="Tag"/>.</param>
        /// <returns><see cref="TimeSeriesId"/>.</returns>
        TimeSeriesId Identify(Source source, Tag tag);
    }
}