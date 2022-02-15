// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
/// runtime service implementations for Heads.
/// </summary>
public class RuntimeServices : ICanBindRuntimeServices
{
    readonly ProjectionsService _projections;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
    /// </summary>
    /// <param name="projections">The <see cref="EventHandlersService"/>.</param>
    public RuntimeServices(ProjectionsService projections)
    {
        _projections = projections;
    }

    /// <inheritdoc/>
    public ServiceAspect Aspect => "Events.Processing";

    /// <inheritdoc/>
    public IEnumerable<Service> BindServices() =>
        new Service[]
        {
            new(_projections, Contracts.Projections.BindService(_projections), Contracts.Projections.Descriptor)
        };
}