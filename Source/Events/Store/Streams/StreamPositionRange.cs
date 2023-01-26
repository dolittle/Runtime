// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents a <see cref="StreamPosition" /> range.
/// </summary>
public record StreamPositionRange(StreamPosition From, ulong Length)
{
    /// <inheritdoc/>
    public override string ToString() => $"({From}, {From + Length})";
}