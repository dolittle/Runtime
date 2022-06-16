// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Execution;

/// <summary>
/// A unique identifier to allow us to trace actions and their consequencies throughout the system.
/// </summary>
public record SpanId(ActivitySpanId Value) : ConceptAs<ActivitySpanId>(Value)
{
    public const string EmptyHexString = "0000000000000000";
    /// <summary>
    /// Represents an Empty <see cref="SpanId" />.
    /// </summary>
    public static readonly SpanId Empty = new (ActivitySpanId.CreateFromBytes(new byte[8]));

    public static implicit operator SpanId(ActivitySpanId id) => new(id);
}
