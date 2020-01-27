// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using Dolittle.Collections;
using Dolittle.Lifecycle;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Represents a <see cref="ICanIdentifyTimeSeries"/> for an well known <see cref="TimeSeriesId"/>.
    /// </summary>
    [Singleton]
    public class TimeSeriesMapIdentifier : ICanIdentifyTimeSeries
    {
        readonly ConcurrentDictionary<Source, ConcurrentDictionary<Tag, TimeSeriesId>> _map = new ConcurrentDictionary<Source, ConcurrentDictionary<Tag, TimeSeriesId>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesMapIdentifier"/> class.
        /// </summary>
        /// <param name="map"><see cref="TimeSeriesMap"/> configuration object.</param>
        public TimeSeriesMapIdentifier(TimeSeriesMap map)
        {
            foreach ((var source, var timeSeriesByTag) in map)
            {
                var tagToTimeSeries = new ConcurrentDictionary<Tag, TimeSeriesId>();
                timeSeriesByTag.ForEach(_ => tagToTimeSeries[_.Key] = _.Value);
                _map[source] = tagToTimeSeries;
            }
        }

        /// <inheritdoc/>
        public bool CanIdentify(Source source, Tag tag)
        {
            if (!_map.ContainsKey(source)) return false;
            return _map[source].ContainsKey(tag);
        }

        /// <inheritdoc/>
        public TimeSeriesId Identify(Source source, Tag tag)
        {
            ThrowIfMissingSource(source);
            ThrowIfTagIsMissingInSource(source, tag);
            return _map[source][tag];
        }

        /// <summary>
        /// Register a mapping between <see cref="Source"/> and <see cref="Tag"/> to <see cref="TimeSeriesId"/>.
        /// </summary>
        /// <param name="source"><see cref="Source"/> to register for.</param>
        /// <param name="tag"><see cref="Tag"/> to register for.</param>
        /// <param name="timeSeriesId"><see cref="TimeSeriesId"/> to register for.</param>
        public void Register(Source source, Tag tag, TimeSeriesId timeSeriesId)
        {
            if (!_map.ContainsKey(source)) _map[source] = new ConcurrentDictionary<Tag, TimeSeriesId>();
            var tagToTimeSeries = _map[source];
            tagToTimeSeries[tag] = timeSeriesId;
        }

        void ThrowIfMissingSource(Source source)
        {
            if (!_map.ContainsKey(source)) throw new MissingSource(source);
        }

        void ThrowIfTagIsMissingInSource(Source source, Tag tag)
        {
            if (!_map[source].ContainsKey(tag)) throw new MissingTagInSource(source, tag);
        }
    }
}