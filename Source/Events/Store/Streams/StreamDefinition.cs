// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents the definition of a Stream.
/// </summary>
public record StreamDefinition(IFilterDefinition FilterDefinition) : IStreamDefinition
{
    /// <inheritdoc/>
    public bool Public => FilterDefinition.Public;

    /// <inheritdoc/>
    public StreamId StreamId => FilterDefinition.TargetStream;

    /// <inheritdoc/>
    public bool Partitioned => FilterDefinition.Partitioned;

    /// <inheritdoc/>
    public override string ToString() => $"Stream Id: {StreamId.Value} Partitioned: {Partitioned} Public: {Public}";
}