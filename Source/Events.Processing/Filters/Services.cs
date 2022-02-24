// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Filters.EventHorizon;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Events.Processing.Filters;

// TODO: Is bindings maybe a better word for these things?
public class Services : ICanAddTenantServices
{
    public void AddFor(TenantId tenant, IServiceCollection services)
    {
        services.AddTransient<Partitioned.FilterProcessor>();
        services.AddTransient<Unpartitioned.FilterProcessor>();
        services.AddTransient<PublicFilterProcessor>();
    }
}
