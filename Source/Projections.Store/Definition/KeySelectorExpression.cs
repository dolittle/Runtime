// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store.Definition;

/// <summary>
/// Represents the projection key selector expression used to point to a property as the key.
/// </summary>
/// <param name="Value">The key selector expression as a string.</param>
public record KeySelectorExpression(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicit operator from string.
    /// </summary>
    /// <param name="key">The projection key.</param>
    public static implicit operator KeySelectorExpression(string key) => new(key);
}
