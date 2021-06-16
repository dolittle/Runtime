// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Autofac;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy
{
    /// <summary>
    /// Represents a system that knows about instances per tenant.
    /// </summary>
    public class InstancesPerTenant
    {
        readonly Dictionary<string, object> _instancesPerKey = new Dictionary<string, object>();
        readonly ITypeActivator _activator;
        readonly ITenantKeyCreator _tenantKeyCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstancesPerTenant"/> class.
        /// </summary>
        /// <param name="tenantKeyCreator"><see cref="ITenantKeyCreator"/> for creating tenant keys.</param>
        /// <param name="activator"><see cref="ITypeActivator"/> to use for activating types into instances.</param>
        public InstancesPerTenant(ITenantKeyCreator tenantKeyCreator, ITypeActivator activator)
        {
            _activator = activator;
            _tenantKeyCreator = tenantKeyCreator;
        }

        /// <summary>
        /// Resolve an instance based on context, binding and service.
        /// </summary>
        /// <param name="context"><see cref="IComponentContext"/> to resolve from.</param>
        /// <param name="binding"><see cref="Binding"/> to resolve.</param>
        /// <param name="service"><see cref="Type">Service type</see> asked for.</param>
        /// <returns>Resolved instance.</returns>
        public object Resolve(IComponentContext context, Binding binding, Type service)
        {
            lock (_instancesPerKey)
            {
                var key = _tenantKeyCreator.GetKeyFor(
                    binding,
                    service);
                if (_instancesPerKey.ContainsKey(key)) return _instancesPerKey[key];

                object instance = null;
                switch (binding.Strategy)
                {
                    case Strategies.Type type:
                        instance = _activator.CreateInstanceFor(context, binding.Service, type.Target);
                        break;

                    case Strategies.Constant constant:
                        instance = constant.Target;
                        break;

                    case Strategies.Callback callback:
                        instance = callback.Target();
                        break;

                    case Strategies.CallbackWithBindingContext callback:
                        instance = callback.Target(new BindingContext(service));
                        break;

                    case Strategies.TypeCallback typeCallback:
                        var typeFromCallback = typeCallback.Target();
                        instance = _activator.CreateInstanceFor(context, binding.Service, typeFromCallback);
                        break;

                    case Strategies.TypeCallbackWithBindingContext typeCallback:
                        var typeFromCallbackWithBindingContext = typeCallback.Target(new BindingContext(service));
                        instance = _activator.CreateInstanceFor(context, binding.Service, typeFromCallbackWithBindingContext);
                        break;
                }

                _instancesPerKey[key] = instance;
                return instance;
            }
        }
    }
}