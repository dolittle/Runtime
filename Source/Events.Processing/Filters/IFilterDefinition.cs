// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines the basis of the definition of a filter.
    /// </summary>
    public interface IFilterDefinition
    {
        /// <summary>
        /// Gets the <see cref="StreamId" /> that the filter filters from.
        /// </summary>
        StreamId SourceStream { get; }

        /// <summary>
        /// Gets the <see cref="StreamId" /> that the filter filters to.
        /// </summary>
        StreamId TargetStream { get; }
    }
}