// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Proto;

namespace Dolittle.Runtime.DependencyInversion.Actors;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets the <see cref="ActorType"/> for the <see cref="Type"/> from its defined attributes.
    /// </summary>
    /// <param name="type">The type to get the actor type for.</param>
    /// <returns>The actor type for the type.</returns>
    public static ActorType GetActorType(this Type type)
        => Attribute.IsDefined(type, typeof(GrainAttribute))
            ? ActorType.Grain
            : typeof(IActor).IsAssignableFrom(type)
                ? ActorType.Actor
                : ActorType.None;
}
