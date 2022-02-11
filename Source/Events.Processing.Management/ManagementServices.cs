// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Processing.Management.EventHandlers;
using Dolittle.Runtime.Events.Processing.Management.Projections;
using Dolittle.Runtime.Management;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Management;

/// <summary>
/// Represents an implementation of <see cref="ICanBindManagementServices"/> that exposes event processing management services.
/// </summary>
public class ManagementServices : ICanBindManagementServices
{
    readonly EventHandlersService _eventHandlers;
    readonly ProjectionsService _projections;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagementServices"/> class.
    /// </summary>
    /// <param name="eventHandlers">The Event Handlers management service.</param>
    /// <param name="projections">The Projections management service.</param>
    public ManagementServices(EventHandlersService eventHandlers, ProjectionsService projections)
    {
        _eventHandlers = eventHandlers;
        _projections = projections;
    }

    /// <inheritdoc />
    public ServiceAspect Aspect => "Events.Processing.Management";

    /// <inheritdoc />
    public IEnumerable<Service> BindServices() =>
        new[]
        {
            new Service(_eventHandlers, Contracts.EventHandlers.BindService(_eventHandlers), Contracts.EventHandlers.Descriptor),
            new Service(_projections, Contracts.Projections.BindService(_projections), Contracts.Projections.Descriptor),
        };
}
