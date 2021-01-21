// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;

namespace Dolittle.Runtime.DependencyInversion.Autofac
{
    /// <summary>
    /// Represents a <see cref="IRegistrationSource"/> that deals with resolving open generic type callbacks.
    /// </summary>
    public class OpenGenericTypeCallbackRegistrationSource : IRegistrationSource
    {
        static readonly IDictionary<Type, Func<IServiceWithType, Type>> _typeCallbackByService = new Dictionary<Type, Func<IServiceWithType, Type>>();

        /// <inheritdoc/>
        public bool IsAdapterForIndividualComponents => false;

        /// <inheritdoc/>
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (!(service is IServiceWithType serviceWithType) ||
                !serviceWithType.ServiceType.IsGenericType ||
                !_typeCallbackByService.ContainsKey(serviceWithType.ServiceType.GetGenericTypeDefinition()))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var serviceOpenGenericType = serviceWithType.ServiceType.GetGenericTypeDefinition();
            var callback = _typeCallbackByService[serviceOpenGenericType];

#pragma warning disable CA2000
            var registration = new ComponentRegistration(
                Guid.NewGuid(),
                new DelegateActivator(serviceWithType.ServiceType, (c, p) => c.ResolveUnregistered(callback(serviceWithType).MakeGenericType(serviceWithType.ServiceType.GetGenericArguments()))),
                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new[] { service },
                new Dictionary<string, object>());
#pragma warning restore CA2000

            return new[] { registration };
        }

        /// <summary>
        /// Add a binding between a <see cref="Type"/> and a <see cref="Func{T, TResult}"/> for resolving from a <see cref="IServiceWithType"/>.
        /// </summary>
        /// <param name="typeCallbackAndServicePair"><see cref="KeyValuePair{TKey, TValue}"/> for the type and resolver.</param>
        internal static void AddService(KeyValuePair<Type, Func<IServiceWithType, Type>> typeCallbackAndServicePair)
        {
            _typeCallbackByService.Add(typeCallbackAndServicePair);
        }
    }
}