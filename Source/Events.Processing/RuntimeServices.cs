// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using Dolittle.Runtime.Events.Processing.EventHorizon;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Services;
using Dolittle.Services;
using grpc = contracts::Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly FiltersService _filtersService;
        readonly EventHandlersService _eventHandlers;
        readonly EventHorizonService _eventHorizon;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="filtersService">The <see cref="FiltersService"/>.</param>
        /// <param name="eventHandlers">The <see cref="EventHandlersService"/>.</param>
        /// <param name="eventHorizon">The <see cref="EventHorizonService" />.</param>
        public RuntimeServices(FiltersService filtersService, EventHandlersService eventHandlers, EventHorizonService eventHorizon)
        {
            _filtersService = filtersService;
            _eventHandlers = eventHandlers;
            _eventHorizon = eventHorizon;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Events.Processing";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_filtersService, grpc.Filters.BindService(_filtersService), grpc.Filters.Descriptor),
                new Service(_eventHandlers, grpc.EventHandlers.BindService(_eventHandlers), grpc.EventHandlers.Descriptor),
                new Service(_eventHorizon, grpc.EventHorizon.BindService(_eventHorizon), grpc.EventHorizon.Descriptor)
            };
        }
    }
}