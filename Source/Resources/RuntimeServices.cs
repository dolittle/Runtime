// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Resources;

/// <summary>
/// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing runtime service implementations for Heads.
/// </summary>
public class RuntimeServices : ICanBindRuntimeServices
{
    readonly ResourcesService _resourcesService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
    /// </summary>
    /// <param name="eventStoreService">The <see cref="ResourcesService"/>.</param>
    public RuntimeServices(ResourcesService eventStoreService)
    {
        _resourcesService = eventStoreService;
    }

    /// <inheritdoc/>
    public ServiceAspect Aspect => "Resources";

    /// <inheritdoc/>
    public IEnumerable<Service> BindServices() =>
        new[]
        {
            new Service(_resourcesService, Contracts.Resources.BindService(_resourcesService), Contracts.Resources.Descriptor)
        };
}