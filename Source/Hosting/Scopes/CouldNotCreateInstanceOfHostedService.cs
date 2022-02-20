// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Hosting.Scopes;

/// <summary>
/// Exception that gets thrown when a <see cref="ServiceDescriptor"/> can not be resolved to an instance of <see cref="IHostedService"/>.
/// </summary>
public class CouldNotCreateInstanceOfHostedService : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotCreateInstanceOfHostedService"/> class.
    /// </summary>
    /// <param name="descriptor">The service descriptor that should be resolvable to a hosted service.</param>
    public CouldNotCreateInstanceOfHostedService(ServiceDescriptor descriptor)
        : base($"The service descriptor {descriptor} could not be resolved to an instance of {nameof(IHostedService)}")
    {
    }
}
