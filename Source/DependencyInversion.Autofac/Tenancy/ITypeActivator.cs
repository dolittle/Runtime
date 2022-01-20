// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy;

/// <summary>
/// Defines a system that can activate.
/// </summary>
public interface ITypeActivator
{
    /// <summary>
    /// Create an instance and fulfill any dependencies for the given context and type.
    /// </summary>
    /// <param name="context">Context requested for activation.</param>
    /// <param name="service">Service definition.</param>
    /// <param name="type">Type to activate.</param>
    /// <returns>An instance of the type.</returns>
    /// <remarks>
    /// Also supports open generic types.
    /// </remarks>
    object CreateInstanceFor(IComponentContext context, Type service, Type type);
}