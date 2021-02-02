// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams.Filters
{
    /// <summary>
    /// Represents a <see cref="IFilterDefinition" /> for a remote filter.
    /// </summary>
    public record FilterDefinition(StreamId SourceStream, StreamId TargetStream, bool Partitioned) : IFilterDefinition
    {
        /// <inheritdoc/>
        public bool Public => false;
    }
}