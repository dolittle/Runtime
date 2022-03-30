// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Scoping;

/// <summary>
/// Indicates that the class should be registered as a per-tenant dependency in a DI container.
/// Meaning that instances will not be shared across execution contexts for different tenants.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GrainAttribute : Attribute
{
    public GrainAttribute(Type actorType)
    {
        ActorType = actorType;
    }

    public Type ActorType { get; }
}
