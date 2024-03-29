// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Actors;

/// <summary>
/// Indicates that the class should be registered as a grain in a DI container per tenant.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TenantGrainAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantGrainAttribute"/>.
    /// </summary>
    /// <param name="actorType">The <see cref="Type"/> of the grain actor.</param>
    /// <param name="clientType">The <see cref="Type"/> of the grain client.</param>
    /// <param name="kind">The kind (type address) of the grain.</param>
    public TenantGrainAttribute(Type actorType, Type clientType, string kind)
    {
        ActorType = actorType;
        ClientType = clientType;
        Kind = kind;
    }
    
    /// <summary>
    /// Gets the <see cref="Type"/> of the grain actor.
    /// </summary>
    public Type ActorType { get; }
    
    /// <summary>
    /// Gets the <see cref="Type"/> of the grain client.
    /// </summary>
    public Type ClientType { get; }
    
    /// <summary>
    /// Gets the kind (type address) of the grain.
    /// </summary>
    public string Kind { get; }
}
