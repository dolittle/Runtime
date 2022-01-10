// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.ApplicationModel;

/// <summary>
/// Represents the concept of a customer.
/// </summary>
public record CustomerId(Guid Value) : ConceptAs<Guid>(Value)
{
    /// <summary>
    /// Represents the identifier for a not set customer.
    /// </summary>
    public static readonly CustomerId NotSet = Guid.Parse("ca900ec9-bae8-462e-b262-fa3efc825ca8");

    /// <summary>
    /// Implicitly converts from a <see cref="Guid"/> to a <see cref="CustomerId"/>.
    /// </summary>
    /// <param name="customer"><see cref="Guid"/> representing the microservice.</param>
    public static implicit operator CustomerId(Guid customer) => new(customer);

    /// <summary>
    /// Create a new <see cref="CustomerId"/> identifier.
    /// </summary>
    /// <returns><see cref="CustomerId"/>.</returns>
    public static CustomerId New() => Guid.NewGuid();
}
