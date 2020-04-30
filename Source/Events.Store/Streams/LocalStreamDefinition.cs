// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines the definition base definition of a Stream.
    /// </summary>
    public class LocalStreamDefinition : IStreamDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalStreamDefinition"/> class.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        public LocalStreamDefinition(IFilterDefinition filterDefinition)
        {
            FilterDefinition = filterDefinition;
        }

        /// <summary>
        /// Gets the <see cref="IFilterDefinition" /> that defines this stream.
        /// </summary>
        public IFilterDefinition FilterDefinition { get; }

        /// <summary>
        /// Gets the <see cref="StreamId" />.
        /// </summary>
        public StreamId StreamId => FilterDefinition.TargetStream;

        /// <inheritdoc/>
        public bool Partitioned => FilterDefinition.Partitioned;

        /// <inheritdoc/>
        public bool Public => FilterDefinition.Public;
    }
}
