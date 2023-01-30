// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents the identification of a stream.
/// </summary>
public record StreamId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Represents the all stream <see cref="StreamId"/>.
    /// </summary>
    public static readonly StreamId EventLog = Guid.Empty;

    /// <summary>
    /// Gets a value indicating whether a <see cref="StreamId" /> is writeable for a user-defined filter.
    /// </summary>
    public bool IsNonWriteable => this == EventLog;

    /// <summary>
    /// Implicitly convert from a <see cref="Guid"/> to a <see cref="StreamId"/>.
    /// </summary>
    /// <param name="streamId"><see cref="Guid"/> representation.</param>
    public static implicit operator StreamId(Guid streamId) => new(streamId);
    
    /// <summary>
    /// Implicitly convert from a <see cref="string"/> to a <see cref="StreamId"/>.
    /// </summary>
    /// <param name="streamId"><see cref="string"/> representation.</param>
    public static implicit operator StreamId(string streamId) => new(Guid.Parse(streamId));

    /// <summary>
    /// Creates a new instance of <see cref="StreamId"/> with a unique id.
    /// </summary>
    /// <returns>A new <see cref="StreamId"/>.</returns>
    public static StreamId New() => Guid.NewGuid();
}
