// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary;

/// <summary>
/// Expresses a Concept as a record for another type, usually a primitive such as Guid, int or string.
/// </summary>
/// <typeparam name="TValue">Type of the concept record.</typeparam>
public record ConceptAs<TValue>(TValue Value)
    where TValue : notnull
{
    /// <summary>
    /// Gets the underlying primitive type of this concept.
    /// </summary>
    public static Type UnderlyingType => typeof(TValue);

    /// <summary>
    /// Implicitly convert from <see cref="ConceptAs{TValue}"/> to type of the <see cref="ConceptAs{TValue}"/>.
    /// </summary>
    /// <param name="value">The converted value.</param>
    public static implicit operator TValue(ConceptAs<TValue> value) => value == null ? default : value.Value;

    /// <inheritdoc/>
    public sealed override string ToString() => Value?.ToString() ?? "NULL";

    /// <inheritdoc/>
    public override int GetHashCode() => HashCodeHelper.Generate(typeof(TValue), Value);
}
