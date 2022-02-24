// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Events.Processing.Projections;

// TODO: Is bindings maybe a better word for these things?
public class Services : ICanAddServices
{
    public void AddTo(IServiceCollection services)
    {
        // TODO: WÆT, shouldn't these have to be registered in the tenant containers? It seems like it works somehow.
        // TODO: If they have tenant-specific dependencies, than this should really be failing.
        services.AddTransient<EventProcessor>();
        services.AddTransient<ProjectionProcessor>();
    }
}
