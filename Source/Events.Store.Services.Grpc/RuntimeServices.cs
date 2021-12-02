// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Store.Services.Grpc;

/// <summary>
/// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
/// runtime service implementations for Heads.
/// </summary>
public class RuntimeServices : ICanBindRuntimeServices
{
    readonly EventStoreGrpcService _eventStoreGrpcService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
    /// </summary>
    /// <param name="eventStoreService">The <see cref="EventStoreService"/>.</param>
    public RuntimeServices(EventStoreGrpcService eventStoreService)
    {
        _eventStoreGrpcService = eventStoreService;
    }

    /// <inheritdoc/>
    public ServiceAspect Aspect => "Events";

    /// <inheritdoc/>
    public IEnumerable<Service> BindServices() =>
        new Service[]
        {
            new(_eventStoreGrpcService, Contracts.EventStore.BindService(_eventStoreGrpcService), Contracts.EventStore.Descriptor)
        };
}