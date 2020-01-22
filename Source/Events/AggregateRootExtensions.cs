// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dolittle.Events
{
    /// <summary>
    /// Extensions for <see cref="AggregateRoot"/>.
    /// </summary>
    public static class AggregateRootExtensions
    {
        /// <summary>
        /// Get handle method from an <see cref="AggregateRoot"/> for a specific <see cref="IEvent"/>, if any.
        /// </summary>
        /// <param name="aggregateRoot"><see cref="AggregateRoot"/> to get method from.</param>
        /// <param name="event"><see cref="IEvent"/> to get method for.</param>
        /// <returns><see cref="MethodInfo"/> containing information about the handle method, null if none exists.</returns>
        public static MethodInfo GetOnMethod(this AggregateRoot aggregateRoot, IEvent @event)
        {
            var eventType = @event.GetType();
            var handleMethods = GetHandleMethodsFor(aggregateRoot.GetType());
            return handleMethods.ContainsKey(eventType) ? handleMethods[eventType] : null;
        }

        /// <summary>
        /// Indicates whether the Aggregate Root maintains state and requires handling events to restore that state.
        /// </summary>
        /// <param name="aggregateRoot"><see cref="AggregateRoot"/> to test for statelessness.</param>
        /// <returns>true if the Aggregate Root does not maintain state.</returns>
        public static bool IsStateless(this AggregateRoot aggregateRoot)
        {
            return GetHandleMethodsFor(aggregateRoot.GetType()).Count == 0;
        }

        static Dictionary<Type, MethodInfo> GetHandleMethodsFor(Type aggregateRootType)
        {
            return typeof(AggregateRootHandleMethods<>)
                              .MakeGenericType(aggregateRootType)
                              .GetRuntimeField("MethodsPerEventType")
                              .GetValue(null) as Dictionary<Type, MethodInfo>;
        }

        static class AggregateRootHandleMethods<T>
        {
            public static readonly Dictionary<Type, MethodInfo> MethodsPerEventType = new Dictionary<Type, MethodInfo>();

            static AggregateRootHandleMethods()
            {
                var aggregateRootType = typeof(T);

                bool hasEventParameter(MethodInfo m)
                {
                    var parameters = m.GetParameters();
                    if (parameters.Length != 1)
                        return false;

                    return typeof(IEvent).GetTypeInfo().IsAssignableFrom(parameters.Single().ParameterType.GetTypeInfo());
                }

                var methods = aggregateRootType.GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.Name.Equals("On", StringComparison.InvariantCultureIgnoreCase) && hasEventParameter(m));
                foreach (var method in methods)
                    MethodsPerEventType[method.GetParameters()[0].ParameterType] = method;
            }
        }
    }
}