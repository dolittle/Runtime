// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace Dolittle.Runtime.DependencyInversion.Autofac
{
    /// <summary>
    /// Extension methods for <see cref="IComponentContext"/>.
    /// </summary>
    /// <remarks>
    /// Taken from https://stackoverflow.com/a/6994144.
    /// </remarks>
    public static class ComponentContextExtensions
    {
        /// <summary>
        /// Resolves an unregistered service.
        /// </summary>
        /// <param name="context">The <see cref="IComponentContext"/>.</param>
        /// <param name="serviceType">Service type.</param>
        /// <param name="parameters"><see cref="IEnumerable{T}"/> of <see cref="Parameter"/>.</param>
        /// <returns>Resolved component.</returns>
        public static object ResolveUnregistered(this IComponentContext context, Type serviceType, IEnumerable<Parameter> parameters)
        {
            var scope = context.Resolve<ILifetimeScope>();
            using var innerScope = scope.BeginLifetimeScope(b => b.RegisterType(serviceType));
            var typedService = new TypedService(serviceType);
            innerScope.ComponentRegistry.TryGetRegistration(typedService, out IComponentRegistration reg);

            var request = new ResolveRequest(typedService, reg, parameters);
            return context.ResolveComponent(request);
        }

        /// <summary>
        /// Resolves an unregistered service.
        /// </summary>
        /// <param name="context">The <see cref="IComponentContext"/>.</param>
        /// <param name="serviceType"><see cref="Type"/> to resolve.</param>
        /// <returns>Resolved service.</returns>
        public static object ResolveUnregistered(this IComponentContext context, Type serviceType)
            => ResolveUnregistered(context, serviceType, Enumerable.Empty<Parameter>());

        /// <summary>
        /// Resolves an unregistered service.
        /// </summary>
        /// <param name="context">The <see cref="IComponentContext"/>.</param>
        /// <param name="serviceType"><see cref="Type"/> to resolve.</param>
        /// <param name="parameters">Params of <see cref="Parameter"/>.</param>
        /// <returns>Resolved service.</returns>
        public static object ResolveUnregistered(this IComponentContext context, Type serviceType, params Parameter[] parameters)
            => ResolveUnregistered(context, serviceType, (IEnumerable<Parameter>)parameters);

        /// <summary>
        /// Resolves an unregistered service.
        /// </summary>
        /// <param name="context">The <see cref="IComponentContext"/>.</param>
        /// <typeparam name="TService"><see cref="Type"/> to resolve.</typeparam>
        /// <returns>Resolved service.</returns>
        public static TService ResolveUnregistered<TService>(this IComponentContext context)
            => (TService)ResolveUnregistered(context, typeof(TService), Enumerable.Empty<Parameter>());

        /// <summary>
        /// Resolves an unregistered service.
        /// </summary>
        /// <param name="context">The <see cref="IComponentContext"/>.</param>
        /// <param name="parameters">Params of <see cref="Parameter"/>.</param>
        /// <typeparam name="TService"><see cref="Type"/> to resolve.</typeparam>
        /// <returns>Resolved service.</returns>
        public static TService ResolveUnregistered<TService>(this IComponentContext context, params Parameter[] parameters)
            => (TService)ResolveUnregistered(context, typeof(TService), (IEnumerable<Parameter>)parameters);
    }
}