// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a system that can add services to a per-tenant <see cref="IServiceCollection"/> for classes with a specific <see cref="Attribute"/>.
/// </summary>
/// <typeparam name="TAttribute">The type of the <see cref="Attribute"/>.</typeparam>
public interface ICanAddTenantServicesForTypesWith<in TAttribute>
    where TAttribute : Attribute
{
    /// <summary>
    /// Adds services for the <see cref="Type"/> with the <typeparamref name="TAttribute"/> attribute for a <see cref="TenantId"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="type">The type with the attribute.</param>
    /// <param name="attribute">The attribute instance.</param>
    /// <param name="tenant">The tenant to add services for.</param>
    /// <param name="services">The service collection to add services into.</param>
    void AddServiceFor(Type type, TAttribute attribute, TenantId tenant, IServiceCollection services);
}
