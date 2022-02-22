// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Resilience;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Streams;

// TODO: Is bindings maybe a better word for these things?
public class Services : ICanAddServices
{
    public void AddTo(IServiceCollection services)
    {
        // TODO: Maybe we want to make an interface for these?
        services.AddTransient<StreamProcessor>();
        services.AddTransient<ScopedStreamProcessor>();
        services.AddTransient<Partitioned.ScopedStreamProcessor>();
        
        // TODO: Fix policy bindings, just now for testing
        services.AddTransient<IAsyncPolicyFor<ICanFetchEventsFromStream>>(provider =>
        {
            var policy = new EventFetcherPolicy(provider.GetRequiredService<ILogger<ICanFetchEventsFromStream>>());
            return new AsyncPolicyFor<ICanFetchEventsFromStream>(policy.Define());
        });
        services.AddTransient<IAsyncPolicyFor<ResilientStreamProcessorStateRepository>>(provider =>
        {
            var policy = new ResilientStreamProcessorStateRepositoryPolicy(provider.GetRequiredService<ILogger<ResilientStreamProcessorStateRepository>>());
            return new AsyncPolicyFor<ResilientStreamProcessorStateRepository>(policy.Define());
        });
    }
}

public class TenantServices : ICanAddTenantServices
{
    public void AddFor(TenantId tenant, IServiceCollection services)
    {
        services.AddTransient<IResilientStreamProcessorStateRepository, ResilientStreamProcessorStateRepository>();
    }
}
