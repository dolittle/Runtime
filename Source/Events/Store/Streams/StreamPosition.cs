// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// An unsigned long used to identify the position of a stream.
/// </summary>
public record StreamPosition(ulong Value) : ConceptAs<ulong>(Value)
{
    /// <summary>
    /// Represents the initial value of the <see cref="StreamPosition">position</see>.
    /// </summary>
    public static readonly StreamPosition Start = 0;

    /// <summary>
    /// Implicitly convert from <see cref="ulong" /> to <see cref="StreamPosition" />.
    /// </summary>
    /// <param name="position">Position number as <see cref="ulong"/>.</param>
    public static implicit operator StreamPosition(ulong position) => new(position);

    /// <summary>
    /// Increments the <see cref="StreamPosition" />.
    /// </summary>
    /// <returns>The new <see cref="StreamPosition" />.</returns>
    public StreamPosition Increment() => Value + 1;
}