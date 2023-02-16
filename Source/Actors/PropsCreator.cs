// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Proto;

namespace Dolittle.Runtime.Actors;

/// <summary>
/// Represents an implementation of <see cref="ICreateProps"/>.
/// </summary>
[Singleton, PerTenant]
public class CreateProps : ICreateProps
{
    readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProps"/> class.
    /// </summary>
    /// <param name="serviceProvider">The tenant specific service provider.</param>
    public CreateProps(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public Props PropsFor<TActor>(params object[] parameters) where TActor : IActor
        => Props.FromProducer(() => ActivatorUtilities.CreateInstance<TActor>(_serviceProvider, parameters));
}
