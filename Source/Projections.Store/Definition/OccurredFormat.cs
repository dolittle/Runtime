// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store.Definition;

/// <summary>
/// Represents the format used to represent the date time mapping of when events occurred to projections.
/// </summary>
/// <param name="Value">The key selector expression as a string.</param>
public record OccurredFormat(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicit operator from string.
    /// </summary>
    /// <param name="key">The occurred format.</param>
    public static implicit operator OccurredFormat(string key) => new(key);
}
