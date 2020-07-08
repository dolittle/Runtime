// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams.Filters
{
    /// <summary>
    /// Defines the basis of the definition of a filter.
    /// </summary>
    public interface IFilterDefinition
    {
        /// <summary>
        /// Gets the <see cref="StreamId" /> to filter from.
        /// </summary>
        StreamId SourceStream { get; }

        /// <summary>
        /// Gets the <see cref="StreamId" /> to filter to.
        /// </summary>
        StreamId TargetStream { get; }

        /// <summary>
        /// Gets a value indicating whether the filter is partitioned or not.
        /// </summary>
        bool Partitioned { get; }

        /// <summary>
        /// Gets a value indicating whether the filter defines a public stream definition.
        /// </summary>
        bool Public { get; }
    }
}
