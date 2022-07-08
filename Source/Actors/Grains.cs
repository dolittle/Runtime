// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Proto.Cluster;

namespace Dolittle.Runtime.Actors;

/// <summary>
/// Represents an implementation of <see cref="ICanAddServicesForTypesWith{TAttribute}"/> that adds services for grains with <see cref="GrainAttribute"/>.
/// </summary>
public class Grains : ICanAddServicesForTypesWith<GrainAttribute>
{
    /// <inheritdoc />
    public void AddServiceFor(Type type, GrainAttribute attribute, IServiceCollection services)
    {
        services.
            AddTransient(
                attribute.ClientType,
                provider => Activator.CreateInstance(
                    attribute.ClientType,
                    provider.GetRequiredService<Cluster>(),
                    "Groot"))
            .AddTransient(type)
            .AddSingleton(new GrainAndActor(type, attribute.ActorType, false));
    }
}
