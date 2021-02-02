// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Reflection;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy
{
    /// <summary>
    /// Represents a <see cref="IRegistrationSource"/> that deals with.
    /// </summary>
    public class BindingsPerTenantsRegistrationSource : IRegistrationSource
    {
        static readonly List<Binding> _bindings = new();
        readonly InstancesPerTenant _instancesPerTenant;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingsPerTenantsRegistrationSource"/> class.
        /// </summary>
        /// <param name="instancesPerTenant">The <see cref="InstancesPerTenant"/>.</param>
        public BindingsPerTenantsRegistrationSource(InstancesPerTenant instancesPerTenant)
            => _instancesPerTenant = instancesPerTenant;

        /// <inheritdoc/>
        public bool IsAdapterForIndividualComponents => false;

        /// <summary>
        /// Add a <see cref="Binding"/> for the registration source to use.
        /// </summary>
        /// <param name="binding"><see cref="Binding"/> to add.</param>
        public static void AddBinding(Binding binding) => _bindings.Add(binding);

        /// <inheritdoc/>
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (!(service is IServiceWithType serviceWithType)) return Enumerable.Empty<IComponentRegistration>();

            if (serviceWithType.ServiceType.HasAttribute<SingletonPerTenantAttribute>() &&
                !HasService(serviceWithType.ServiceType) &&
                !IsGenericAndHasGenericService(serviceWithType.ServiceType))
            {
                AddBinding(new Binding(serviceWithType.ServiceType, new Strategies.Type(serviceWithType.ServiceType), new Scopes.Transient()));
            }

            if (serviceWithType == null ||
                (!HasService(serviceWithType.ServiceType) &&
                !IsGenericAndHasGenericService(serviceWithType.ServiceType)))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var registration = new ComponentRegistration(
                Guid.NewGuid(),
#pragma warning disable CA2000
                new DelegateActivator(
                    serviceWithType.ServiceType,
                    (c, p) =>
                        _instancesPerTenant.Resolve(c, GetBindingFor(serviceWithType.ServiceType), serviceWithType.ServiceType)),
#pragma warning restore CA2000

                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new[] { service },
                new Dictionary<string, object>());

            return new[] { registration };
        }

        static bool HasService(Type service) => _bindings.Any(_ => _.Service == service);

        static bool IsGenericAndHasGenericService(Type service)
            => service.IsGenericType
                && _bindings.Any(_ => _.Service == service.GetGenericTypeDefinition());

        static Binding GetBindingFor(Type service)
        {
            var binding = _bindings.SingleOrDefault(_ => _.Service == service);
            if (binding == null && service.IsGenericType) binding = _bindings.Single(_ => _.Service == service.GetGenericTypeDefinition());
            if (binding == null) throw new UnableToFindBindingForService(service);
            return binding;
        }
    }
}
