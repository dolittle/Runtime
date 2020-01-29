// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Defines a system that can be used to identify a <see cref="TimeSeriesId"/> from a given.
    /// <see cref="Source"/> and <see cref="Tag"/>.
    /// </summary>
    public interface ICanIdentifyTimeSeries
    {
        /// <summary>
        /// Check if the given <see cref="Source"/> and <see cref="Tag"/> can be identified.
        /// </summary>
        /// <param name="source"><see cref="Source"/> to check.</param>
        /// <param name="tag"><see cref="Tag"/> to check.</param>
        /// <returns>True if identifiable, false if not.</returns>
        bool CanIdentify(Source source, Tag tag);

        /// <summary>
        /// Identify the given <see cref="Source"/> and <see cref="Tag"/> can be identified.
        /// </summary>
        /// <param name="source"><see cref="Source"/> to identify.</param>
        /// <param name="tag"><see cref="Tag"/> to identify.</param>
        /// <returns>Identified <see cref="TimeSeriesId"/>.</returns>
        TimeSeriesId Identify(Source source, Tag tag);
    }
}