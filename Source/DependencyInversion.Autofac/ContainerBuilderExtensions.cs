// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.DependencyInversion.Autofac.Tenancy;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Reflection;

namespace Dolittle.Runtime.DependencyInversion.Autofac
{
    /// <summary>
    /// Extensions for <see cref="ContainerBuilder"/>.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Add Dolittle specifics to the <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <param name="containerBuilder"><see cref="ContainerBuilder"/> to extend.</param>
        /// <param name="assemblies">Discovered <see cref="IAssemblies"/>.</param>
        /// <param name="bindings"><see cref="IBindingCollection">Bindings</see> to hook up.</param>
        public static void AddDolittle(this ContainerBuilder containerBuilder, IAssemblies assemblies, IBindingCollection bindings)
        {
            var allAssemblies = assemblies.GetAll().ToArray();
            containerBuilder.RegisterAssemblyModules(allAssemblies);

            var selfBindingRegistrationSource = CreateSelfBindingRegistrationSource();

            containerBuilder.AddBindingsPerTenantRegistrationSource();

            RegisterWellKnownSources(containerBuilder, selfBindingRegistrationSource);
            RegisterWellKnownModules(containerBuilder);
            DiscoverAndRegisterRegistrationSources(containerBuilder, allAssemblies);
            RegisterUpBindingsIntoContainerBuilder(bindings, containerBuilder);
        }

        static SelfBindingRegistrationSource CreateSelfBindingRegistrationSource()
            => new(
                type =>
                    (!type.Namespace.StartsWith("Microsoft", StringComparison.InvariantCulture) &&
                    !type.Namespace.StartsWith("System", StringComparison.InvariantCulture)) ||
                    type.Namespace.StartsWith("Microsoft.Extensions.Logging", StringComparison.InvariantCulture))
            {
                RegistrationConfiguration = HandleLifeCycleFor
            };

        static void RegisterWellKnownModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule(new LoggerModule());
        }

        static void RegisterWellKnownSources(ContainerBuilder containerBuilder, SelfBindingRegistrationSource selfBindingRegistrationSource)
        {
            containerBuilder.RegisterSource(selfBindingRegistrationSource);
            containerBuilder.RegisterSource(new FactoryForRegistrationSource());
            containerBuilder.RegisterSource(new OpenGenericCallbackRegistrationSource());
            containerBuilder.RegisterSource(new OpenGenericTypeCallbackRegistrationSource());
        }

        static void HandleLifeCycleFor(IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder)
        {
            var service = builder.RegistrationData.Services.First();
            if (service is TypedService)
            {
                var typedService = service as TypedService;
                if (typedService.ServiceType.HasAttribute<SingletonAttribute>()) builder.SingleInstance();
            }
        }

        static void RegisterUpBindingsIntoContainerBuilder(IBindingCollection bindings, ContainerBuilder containerBuilder)
        {
            bindings.ForEach(binding =>
            {
                if (binding.Scope is Scopes.SingletonPerTenant)
                {
                    BindingsPerTenantsRegistrationSource.AddBinding(binding);
                    return;
                }

                if (binding.Service.ContainsGenericParameters)
                {
                    switch (binding.Strategy)
                    {
                        case Strategies.Type type:
                            {
                                var registrationBuilder = containerBuilder.RegisterGeneric(type.Target)
                                    .AsSelf()
                                    .As(binding.Service);
                                if (binding.Scope is Scopes.Singleton) registrationBuilder = registrationBuilder.SingleInstance();
                            }

                            break;

                        case Strategies.Callback callback:
                            {
                                OpenGenericCallbackRegistrationSource.AddService(new KeyValuePair<Type, Func<IServiceWithType, object>>(binding.Service, _ => callback.Target()));
                            }

                            break;

                        case Strategies.CallbackWithBindingContext callback:
                            {
                                OpenGenericCallbackRegistrationSource.AddService(new KeyValuePair<Type, Func<IServiceWithType, object>>(binding.Service, (serviceWithType) => callback.Target(new BindingContext(serviceWithType.ServiceType))));
                            }

                            break;

                        case Strategies.TypeCallback callback:
                            {
                                OpenGenericTypeCallbackRegistrationSource.AddService(new KeyValuePair<Type, Func<IServiceWithType, Type>>(binding.Service, _ => callback.Target()));
                            }

                            break;

                        case Strategies.TypeCallbackWithBindingContext callback:
                            {
                                OpenGenericTypeCallbackRegistrationSource.AddService(new KeyValuePair<Type, Func<IServiceWithType, Type>>(binding.Service, (serviceWithType) => callback.Target(new BindingContext(serviceWithType.ServiceType))));
                            }

                            break;
                    }
                }
                else
                {
                    switch (binding.Strategy)
                    {
                        case Strategies.Type type:
                            {
                                var registrationBuilder = containerBuilder.RegisterType(type.Target)
                                    .AsSelf()
                                    .As(binding.Service);
                                if (binding.Scope is Scopes.Singleton) registrationBuilder = registrationBuilder.SingleInstance();
                            }

                            break;

                        case Strategies.Constant constant:
                            containerBuilder.RegisterInstance(constant.Target).As(binding.Service);
                            break;

                        case Strategies.Callback callback:
                            {
                                var registrationBuilder = containerBuilder.Register(_ => callback.Target()).As(binding.Service);
                                if (binding.Scope is Scopes.Singleton) registrationBuilder = registrationBuilder.SingleInstance();
                            }

                            break;

                        case Strategies.CallbackWithBindingContext callback:
                            {
                                var registrationBuilder = containerBuilder.Register(_ => callback.Target(new BindingContext(binding.Service))).As(binding.Service);
                                if (binding.Scope is Scopes.Singleton) registrationBuilder = registrationBuilder.SingleInstance();
                            }

                            break;

                        case Strategies.TypeCallback typeCallback:
                            {
                                var registrationBuilder = containerBuilder.Register(_ => _.Resolve(typeCallback.Target())).As(binding.Service);
                                if (binding.Scope is Scopes.Singleton) registrationBuilder = registrationBuilder.SingleInstance();
                            }

                            break;

                        case Strategies.TypeCallbackWithBindingContext typeCallback:
                            {
                                var registrationBuilder = containerBuilder.Register(_ => _.Resolve(typeCallback.Target(new BindingContext(binding.Service)))).As(binding.Service);
                                if (binding.Scope is Scopes.Singleton) registrationBuilder = registrationBuilder.SingleInstance();
                            }

                            break;
                    }
                }
            });
        }

        static void DiscoverAndRegisterRegistrationSources(ContainerBuilder containerBuilder, IEnumerable<Assembly> allAssemblies)
        {
            allAssemblies.ForEach(assembly =>
            {
                var registrationSourceProviderTypes = assembly.GetTypes().Where(type => type.HasInterface<ICanProvideRegistrationSources>());
                registrationSourceProviderTypes.ForEach(registrationSourceProviderType =>
                {
                    ThrowIfRegistrationSourceProviderTypeIsMissingDefaultConstructor(registrationSourceProviderType);
                    var registrationSourceProvider = Activator.CreateInstance(registrationSourceProviderType) as ICanProvideRegistrationSources;
                    var registrationSources = registrationSourceProvider.Provide();
                    registrationSources.ForEach(_ => containerBuilder.RegisterSource(_));
                });
            });
        }

        static void ThrowIfRegistrationSourceProviderTypeIsMissingDefaultConstructor(Type type)
        {
            if (!type.HasDefaultConstructor()) throw new RegistrationSourceProviderMustHaveADefaultConstructor(type);
        }
    }
}