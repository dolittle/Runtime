// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Scoping;

/// <summary>
/// Indicates that the class should be registered as a grain in a DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GrainAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GrainAttribute"/>.
    /// </summary>
    /// <param name="actorType">The <see cref="Type"/> of the grain actor.</param>
    public GrainAttribute(Type actorType)
    {
        ActorType = actorType;
    }
    
    /// <summary>
    /// Gets the <see cref="Type"/> of the grain actor.
    /// </summary>
    public Type ActorType { get; }
}
