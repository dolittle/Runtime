// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.MongoDB.EventHorizon;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Events.Store.MongoDB;

// TODO: Is bindings maybe a better word for these things?
public class Services : ICanAddTenantServices
{
    public void AddFor(TenantId tenant, IServiceCollection services)
    {
        services.AddTransient<IWriteEventsToStreams, EventsToStreamsWriter>();
        services.AddTransient<IWriteEventsToStreamCollection, EventsToStreamsWriter>();
        services.AddTransient<IWriteEventsToPublicStreams, EventsToPublicStreamsWriter>();
    }
}
