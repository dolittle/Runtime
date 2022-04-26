// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Server.Bootstrap;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Represents an implementation of <see cref="ICanPerformBoostrapProcedure"/> for bootstrapping the event store actors. 
/// </summary>
[Singleton]
public class BootstrapProcedures : ICanPerformBoostrapProcedure
{
    readonly ActorSystem _actorSystem;
    readonly Func<TenantId, ICreateProps> _getPropsCreator;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BootstrapProcedures"/> class.
    /// </summary>
    /// <param name="actorSystem"></param>
    /// <param name="getPropsCreator"></param>
    public BootstrapProcedures(ActorSystem actorSystem, Func<TenantId, ICreateProps> getPropsCreator)
    {
        _actorSystem = actorSystem;
        _getPropsCreator = getPropsCreator;
    }

    /// <inheritdoc />
    public Task Perform() => Task.CompletedTask;

    /// <inheritdoc />
    public Task PerformForTenant(TenantId tenant)
    {
        _actorSystem.Root.SpawnNamed(_getPropsCreator(tenant).PropsFor<EventStoreCatchupActor>(), EventStoreCatchupActor.GetActorName(tenant));
        return Task.CompletedTask;
    }
}
