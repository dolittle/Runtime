// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents an implentation of <see cref="IStreamDefinition" /> which represents the definition of the Event Log.
/// </summary>
public record EventLogStreamDefinition : IStreamDefinition
{
    /// <inheritdoc/>
    public StreamId StreamId => StreamId.EventLog;

    /// <inheritdoc/>
    public bool Partitioned => false;

    /// <inheritdoc/>
    public bool Public => false;

    /// <inheritdoc/>
    public IFilterDefinition FilterDefinition => default;
}