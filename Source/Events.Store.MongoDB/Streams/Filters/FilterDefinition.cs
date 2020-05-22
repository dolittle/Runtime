// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters
{
    /// <summary>
    /// Represents a persisted <see cref="Store.Streams.Filters.FilterDefinition" />.
    /// </summary>
    public class FilterDefinition : AbstractFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterDefinition"/> class.
        /// </summary>
        /// <param name="filterId">The filter id.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="partitioned">Whether the filter is partitioned or not.</param>
        public FilterDefinition(Guid filterId, Guid sourceStream, bool partitioned)
            : base(filterId, sourceStream, partitioned, false)
        {
        }

        /// <summary>
        /// Converts the stored filter into the runtime <see cref="IFilterDefinition" /> that it represents.
        /// </summary>
        /// <returns>The runtime <see cref="IFilterDefinition" />.</returns>
        public override IFilterDefinition AsRuntimeRepresentation() => new Store.Streams.Filters.FilterDefinition(SourceStream, FilterId, Partitioned);
    }
}
