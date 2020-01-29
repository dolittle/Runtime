// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Types;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Represents an implementation of <see cref="ITimeSeriesMapper"/>.
    /// </summary>
    public class TimeSeriesMapper : ITimeSeriesMapper
    {
        readonly IInstancesOf<ICanIdentifyTimeSeries> _identifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesMapper"/> class.
        /// </summary>
        /// <param name="identifiers"><see cref="IInstancesOf{T}"/> of <see cref="ICanIdentifyTimeSeries"/>.</param>
        public TimeSeriesMapper(IInstancesOf<ICanIdentifyTimeSeries> identifiers)
        {
            _identifiers = identifiers;
        }

        /// <inheritdoc/>
        public TimeSeriesId Identify(Source source, Tag tag)
        {
            var identifiers = _identifiers.Where(_ => _.CanIdentify(source, tag)).ToArray();
            ThrowIfAmbiguousTimeSeriesIdentifiers(source, tag, identifiers);
            ThrowIfNoTimeSeriesIdentifierCanIdentify(source, tag, identifiers);
            return identifiers[0].Identify(source, tag);
        }

        /// <inheritdoc/>
        public bool CanIdentify(Source source, Tag tag)
        {
            var identifiers = _identifiers.Where(_ => _.CanIdentify(source, tag)).ToArray();
            ThrowIfAmbiguousTimeSeriesIdentifiers(source, tag, identifiers);
            return identifiers.Length > 0;
        }

        void ThrowIfAmbiguousTimeSeriesIdentifiers(Source source, Tag tag, IEnumerable<ICanIdentifyTimeSeries> identifiers)
        {
            if (identifiers.Count() > 1) throw new AmbiguousTimeSeriesIdentifiers(source, tag, identifiers);
        }

        void ThrowIfNoTimeSeriesIdentifierCanIdentify(Source source, Tag tag, IEnumerable<ICanIdentifyTimeSeries> identifiers)
        {
            if (!identifiers.Any()) throw new NoTimeSeriesIdentifierCanIdentify(source, tag);
        }
    }
}