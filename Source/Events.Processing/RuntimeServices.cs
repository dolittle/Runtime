// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
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
        readonly ScopedEventHandlersService _scopedEventHandlers;
        readonly ScopedFiltersService _scopedFilters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="filtersService">The <see cref="FiltersService"/>.</param>
        /// <param name="eventHandlers">The <see cref="EventHandlersService"/>.</param>
        /// <param name="scopedEventHandlers">The <see cref="ScopedEventHandlersService" />.</param>
        /// <param name="scopedFilters">The <see cref="ScopedFiltersService" />.</param>
        public RuntimeServices(FiltersService filtersService, EventHandlersService eventHandlers, ScopedEventHandlersService scopedEventHandlers, ScopedFiltersService scopedFilters)
        {
            _filtersService = filtersService;
            _eventHandlers = eventHandlers;
            _scopedEventHandlers = scopedEventHandlers;
            _scopedFilters = scopedFilters;
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
                new Service(_scopedEventHandlers, grpc.ScopedEventHandlers.BindService(_scopedEventHandlers), grpc.ScopedEventHandlers.Descriptor),
                new Service(_scopedFilters, grpc.ScopedFilters.BindService(_scopedFilters), grpc.ScopedFilters.Descriptor),
            };
        }
    }
}