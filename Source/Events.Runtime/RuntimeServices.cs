// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using Dolittle.Runtime.Heads;
using Dolittle.Services;
using grpc = contracts::Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Events.Runtime
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly EventStoreService _eventStoreService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="eventStoreService">The <see cref="EventStoreService"/>.</param>
        public RuntimeServices(EventStoreService eventStoreService)
        {
            _eventStoreService = eventStoreService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Events";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_eventStoreService, grpc.EventStore.BindService(_eventStoreService), grpc.EventStore.Descriptor)
            };
        }
    }
}