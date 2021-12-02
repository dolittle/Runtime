// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
/// runtime service implementations for Heads.
/// </summary>
public class RuntimeServices : ICanBindRuntimeServices
{
    readonly EventHandlersService _eventHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
    /// </summary>
    /// <param name="eventHandlers">The <see cref="EventHandlersService"/>.</param>
    public RuntimeServices(EventHandlersService eventHandlers)
    {
        _eventHandlers = eventHandlers;
    }

    /// <inheritdoc/>
    public ServiceAspect Aspect => "Events.Processing";

    /// <inheritdoc/>
    public IEnumerable<Service> BindServices() =>
        new Service[]
        {
            new(_eventHandlers, Contracts.EventHandlers.BindService(_eventHandlers), Contracts.EventHandlers.Descriptor)
        };
}