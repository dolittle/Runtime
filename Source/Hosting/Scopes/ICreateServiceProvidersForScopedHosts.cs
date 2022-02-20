// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Hosting.Scopes;

/// <summary>
/// Defines a system that can create an instance of <see cref="IServiceProvider"/> for each <see cref="ScopedHostDefinition"/>.
/// </summary>
public interface ICreateServiceProvidersForScopedHosts
{
    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> for the provided <see cref="ScopedHostDefinition"/>.
    /// </summary>
    /// <remarks>
    /// One instance will be created per host.
    /// </remarks>
    /// <param name="host">The scoped host definition to create a service provider for.</param>
    /// <returns>The created service provider for the scoped host.</returns>
    IServiceProvider GetServiceProviderFor(ScopedHostDefinition host);
}
