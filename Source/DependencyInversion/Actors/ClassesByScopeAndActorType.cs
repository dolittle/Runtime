// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion.Actors;

/// <summary>
/// Represents a set of discovered classes grouped by DI scoping and actor type.
/// </summary>
/// <param name="Global">The discovered classes that should be registered in the global container.</param>
/// <param name="PerTenant">The discovered classes that should be registered in the per-tenant containers.</param>
public record ClassesByScopeAndActorType(
    ClassesByActorType Global,
    ClassesByActorType PerTenant);
