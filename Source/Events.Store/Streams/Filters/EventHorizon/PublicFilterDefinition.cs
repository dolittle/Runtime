// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;

/// <summary>
/// Represents an implementation of <see cref="IFilterDefinition" /> for a public filter.
/// </summary>
public record PublicFilterDefinition(StreamId SourceStream, StreamId TargetStream) : IFilterDefinition
{
    /// <inheritdoc/>
    public bool Partitioned => true;

    /// <inheritdoc/>
    public bool Public => true;
}