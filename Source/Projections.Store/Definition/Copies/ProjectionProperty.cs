// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies;

/// <summary>
/// Represents a property in a Projection read model.
/// </summary>
/// <param name="Value"></param>
public record ProjectionProperty(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly convert from a <see cref="string"/> to a <see cref="ProjectionProperty"/>.
    /// </summary>
    /// <param name="name">The <see cref="string"/> representation of the projection property.</param>
    /// <returns>The converted <see cref="ProjectionProperty"/> concept.</returns>
    public static implicit operator ProjectionProperty(string name) => new(name);
}
