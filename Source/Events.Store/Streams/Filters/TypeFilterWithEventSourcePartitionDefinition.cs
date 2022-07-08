// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.Streams.Filters;

/// <summary>
/// Represents a <see cref="FilterDefinition" /> for type filter with event source partition.
/// </summary>
public record TypeFilterWithEventSourcePartitionDefinition(
    StreamId SourceStream,
    StreamId TargetStream,
    IEnumerable<ArtifactId> Types,
    bool Partitioned) : IFilterDefinition
{

    /// <inheritdoc/>
    public bool Public => false;
}