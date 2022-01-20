// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Management;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="ICanBindManagementServices"/> that exposes EventHandler management services.
/// </summary>
public class ManagementServices : ICanBindManagementServices
{
    readonly EventHandlersService _eventHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagementServices"/> class.
    /// </summary>
    /// <param name="eventHandlers">The <see cref="EventHandlersService"/>.</param>
    public ManagementServices(EventHandlersService eventHandlers)
    {
        _eventHandlers = eventHandlers;
    }

    /// <inheritdoc />
    public ServiceAspect Aspect => "Events.Processing.Management";

    /// <inheritdoc />
    public IEnumerable<Service> BindServices() =>
        new[]
        {
            new Service(_eventHandlers, Contracts.EventHandlers.BindService(_eventHandlers), Contracts.EventHandlers.Descriptor),
        };
}