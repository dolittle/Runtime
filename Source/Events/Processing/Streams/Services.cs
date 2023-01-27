// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;

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
    }
}

public class TenantServices : ICanAddTenantServices
{
    public void AddFor(TenantId tenant, IServiceCollection services)
    {
        // services.AddTransient<IStreamProcessorStates, ResilientStreamProcessorStateRepository>();
    }
}
