// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy;

/// <summary>
/// Defines a system that can create unique keys for a given binding and service in a tenant context.
/// </summary>
public interface ITenantKeyCreator
{
    /// <summary>
    /// Get key for a binding and service for the current tenant.
    /// </summary>
    /// <param name="binding"><see cref="Binding"/> to get for.</param>
    /// <param name="service"><see cref="Type">Service</see> to get for.</param>
    /// <returns>A key for the given context.</returns>
    string GetKeyFor(Binding binding, Type service);
}