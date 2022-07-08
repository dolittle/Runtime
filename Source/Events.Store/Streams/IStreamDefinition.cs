// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines the definition base definition of a Stream.
/// </summary>
public interface IStreamDefinition
{
    /// <summary>
    /// Gets the <see cref="Streams.StreamId" />.
    /// </summary>
    StreamId StreamId { get; }

    /// <summary>
    /// Gets a value indicating whether the stream is partitioned.
    /// </summary>
    bool Partitioned { get; }

    /// <summary>
    /// Gets a value indicating whether this is a public stream.
    /// </summary>
    bool Public { get; }

    /// <summary>
    /// Gets the <see cref="IFilterDefinition" /> that creates the Stream.
    /// </summary>
    IFilterDefinition FilterDefinition { get; }
}