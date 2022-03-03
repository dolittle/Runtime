// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Domain.Platform;

/// <summary>
/// Represents the name of a <see cref="CustomerId"/>.
/// </summary>
public record CustomerName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Gets the <see cref="CustomerName"/> representing an undefined name.
    /// </summary>
    public static readonly CustomerName NotSet = "[Not Set]";

    /// <summary>
    /// Implicitly converts from a <see cref="string"/> to a <see cref="CustomerName"/>.
    /// </summary>
    /// <param name="customerName">Name of the <see cref="CustomerId"/>.</param>
    public static implicit operator CustomerName(string customerName) => new(customerName);
}
