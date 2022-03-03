// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Dolittle.Runtime.Domain.Tenancy;

namespace Dolittle.Runtime.DependencyInversion.Tenancy;

/// <summary>
/// Extension methods for <see cref="ContainerBuilder"/>.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Adds registrations for dependencies of type <see cref="Func{TResult}"/> where the first parameter is a <see cref="TenantId"/>.
    /// The resolving is delegated to the per-tenant containers using the rest of the parameters.
    /// </summary>
    /// <param name="builder">The container builder to add the registrations to.</param>
    /// <returns>The container builder for continuation.</returns>
    public static ContainerBuilder AddTenantFactories(this ContainerBuilder builder)
    {
        builder.RegisterSource<GeneratedTenantFactoryRegistrationSource>();
        return builder;
    }
}
