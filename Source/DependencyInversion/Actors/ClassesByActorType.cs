// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Actors;

/// <summary>
/// Represents a set of discovered classes grouped by actor type
/// </summary>
/// <param name="Actor">The discovered classes that should be registered as actors.</param>
/// <param name="Grain">The discovered classes that should be registered as grains.</param>
public record ClassesByActorType(
    Type[] Actor,
    GrainAndActor[] Grain);
