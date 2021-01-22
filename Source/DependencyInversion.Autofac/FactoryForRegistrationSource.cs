// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;

namespace Dolittle.Runtime.DependencyInversion.Autofac
{
    /// <summary>
    /// Represents a <see cref="IRegistrationSource"/> that deals with resolving <see cref="FactoryFor{T}"/>.
    /// </summary>
    public class FactoryForRegistrationSource : IRegistrationSource
    {
        /// <inheritdoc/>
        public bool IsAdapterForIndividualComponents => false;

        /// <inheritdoc/>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (!(service is IServiceWithType serviceWithType) ||
                !serviceWithType.ServiceType.IsGenericType ||
                serviceWithType.ServiceType != typeof(FactoryFor<>).MakeGenericType(serviceWithType.ServiceType.GetGenericArguments()[0]))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var registration = new ComponentRegistration(
                Guid.NewGuid(),
#pragma warning disable CA2000
                new DelegateActivator(serviceWithType.ServiceType, (c, _) =>
                {
                    var container = c.Resolve<IContainer>();
                    var typeForFactory = serviceWithType.ServiceType.GetGenericArguments()[0];
                    var wrapperType = typeof(FactoryForClass<>).MakeGenericType(typeForFactory);
                    var containerField = wrapperType.GetField("Container");
                    containerField.SetValue(null, container);
                    var activateMethod = wrapperType.GetMethod("Activate").MakeGenericMethod(typeForFactory);
                    return activateMethod.CreateDelegate(serviceWithType.ServiceType);
                }),
#pragma warning restore CA2000
                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new[] { service },
                new Dictionary<string, object>());

            return new[] { registration };
        }

        static class FactoryForClass<T>
        {
            public static IContainer Container = null;

            public static TType Activate<TType>()
            {
                return Container.Get<TType>();
            }
        }
    }
}