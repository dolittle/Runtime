// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Represents the name of the collection to store MongoDB read model copies in.
/// </summary>
/// <param name="Value">The name of the collection.</param>
public record CollectionName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Gets the <see cref="CollectionName"/> value when it is not set.
    /// </summary>
    public static readonly CollectionName NotSet = "Not Set";

    /// <summary>
    /// Implicitly convert from a <see cref="string"/> to a <see cref="CollectionName"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> representation of the collection name.</param>
    /// <returns>The converted <see cref="CollectionName"/> concept.</returns>
    public static implicit operator CollectionName(string name) => new(name);
}
