// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.EventHandlers.Actors;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Events;

public class ActorPropsAdder : ICanAddServices
{
    public void AddTo(IServiceCollection services)
    {
        services.AddSingleton<CreateStreamProcessorActorProps>(sp => EventHandlerProcessorActor.CreateFactory(new CreateProps(sp)));
        services.AddScoped<Func<TenantId, CreateTenantScopedStreamProcessorProps>>(globalServiceProvider =>
            tenant =>
            {
                var createProps = globalServiceProvider.GetRequiredService<Func<TenantId, ICreateProps>>()(tenant);

                // var getTenantedServiceProvider = globalServiceProvider.GetRequiredService<Func<TenantId, IServiceProvider>>();
                // var serviceProvider = getTenantedServiceProvider(tenant);
                // var createProps = new CreateProps(serviceProvider);
                return TenantScopedStreamProcessorActor.CreateFactory(createProps);
            });
    }
}
