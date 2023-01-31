// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents the information for a <see cref="Projection"/>.
/// </summary>
/// <param name="Definition">The definition of the Projection.</param>
/// <param name="HasAlias">Whether or not an alias was provided by the Client for the Projection.</param>
/// <param name="Alias">The alias of the Projection.</param>
public record ProjectionInfo(
    ProjectionDefinition Definition,
    bool HasAlias,
    ProjectionAlias Alias);
