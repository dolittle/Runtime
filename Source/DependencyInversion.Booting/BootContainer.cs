// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Booting;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Logging;

namespace Dolittle.Runtime.DependencyInversion.Booting
{
    /// <summary>
    /// Represents a <see cref="IContainer"/> used during booting.
    /// </summary>
    public class BootContainer : IContainer
    {
        static IContainer _container;
        readonly IDictionary<Type, IActivationStrategy> _bindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootContainer"/> class.
        /// </summary>
        /// <param name="bindings"><see cref="IBindingCollection">Bindings</see> for the <see cref="BootContainer"/>.</param>
        /// <param name="newBindingsNotifier"><see cref="ICanNotifyForNewBindings">Notifier</see> of new <see cref="Binding">bindings</see>.</param>
        public BootContainer(IBindingCollection bindings, ICanNotifyForNewBindings newBindingsNotifier)
        {
            _bindings = bindings.ToDictionary(_ => _.Service, _ => _.Strategy);

            _bindings[typeof(IContainer)] = new Strategies.Constant(this);
            _bindings[typeof(GetContainer)] = new Strategies.Constant((GetContainer)(() => this));

            newBindingsNotifier.SubscribeTo(_ => _.ToDictionary(_ => _.Service, _ => _.Strategy).ForEach(_bindings.Add));
        }

        /// <inheritdoc/>
        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        /// <inheritdoc/>
        public object Get(Type type)
        {
            if (_container != null && _container.GetType() != typeof(BootContainer)) return _container.Get(type);

            if (_bindings.TryGetValue(type, out var strategyForType))
                return InstantiateBinding(strategyForType, type);

            if (type.IsGenericType && _bindings.TryGetValue(type.GetGenericTypeDefinition(), out var strategyForOpenGenericType))
                return InstantiateBinding(strategyForOpenGenericType, type);

            if (type.IsInterface)
                throw new TypeNotBoundInContainer(type, _bindings.Select(_ => _.Key));

            return Create(type);
        }

        /// <summary>
        /// Method that gets called when <see cref="IContainer"/> is ready.
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> instance.</param>
        internal static void ContainerReady(IContainer container)
        {
            _container = container;
        }

        object InstantiateBinding(IActivationStrategy strategy, Type type) => strategy switch
        {
            Strategies.Constant constant => constant.Target,
            Strategies.Callback callback => callback.Target(),
            Strategies.CallbackWithBindingContext callback => callback.Target(new BindingContext(type)),
            Strategies.Type typeConstant => Get(typeConstant.Target),
            Strategies.TypeCallback typeCallback => Get(typeCallback.Target()),
            Strategies.TypeCallbackWithBindingContext typeCallback => Get(typeCallback.Target(new BindingContext(type))),
            _ => null
        };

        object Create(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length == 0) return Activator.CreateInstance(type);
            if (constructors.Length > 1) throw new OnlySingleConstructorSupported(type);
            var constructor = constructors[0];

            var parameters = constructor.GetParameters().Select(parameter =>
            {
                try
                {
                    if (parameter.ParameterType == typeof(ILogger))
                        return Get(typeof(ILogger<>).MakeGenericType(type));

                    return Get(parameter.ParameterType);
                }
                catch (TypeNotBoundInContainer)
                {
                    throw new ConstructorDependencyNotSupported(type, parameter.ParameterType, _bindings.Select(_ => _.Key));
                }
            }).ToArray();

            return constructor.Invoke(parameters);
        }
    }
}
