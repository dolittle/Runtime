// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Hosting.Scopes;

/// <summary>
/// Represents the definition of a scoped host.
/// </summary>
public class ScopedHostDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedHostDefinition"/> class.
    /// </summary>
    /// <param name="scopedServices">The services that should be available in the context of the scoped host.</param>
    public ScopedHostDefinition(IServiceCollection scopedServices)
    {
        ScopedServices = scopedServices;
    }

    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> that should be made available in the context of the scoped host.
    /// </summary>
    public IServiceCollection ScopedServices { get; }
}
