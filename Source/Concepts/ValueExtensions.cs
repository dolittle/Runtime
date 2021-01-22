// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Runtime.Reflection;

namespace Dolittle.Runtime.Concepts
{
    /// <summary>
    /// Provides extensions related to <see cref="Type">types</see> and others related to <see cref="Value{T}"/>.
    /// </summary>
    public static class ValueExtensions
    {
        /// <summary>
        /// Check if a type is a Value{T} or not.
        /// </summary>
        /// <param name="objectType"><see cref="Type"/> to check.</param>
        /// <returns>True if type is a Value, false if not.</returns>
        public static bool IsValue(this Type objectType)
        {
            return objectType.IsDerivedFromOpenGeneric(typeof(Value<>));
        }

        /// <summary>
        /// Check if an object is an instance of a Value{T} or not.
        /// </summary>
        /// <param name="instance">instance to check.</param>
        /// <returns>True if object is a Value{T}, false if not.</returns>
        public static bool IsValue(this object instance)
        {
            return IsValue(instance.GetType());
        }

        /// <summary>
        /// Gets the closing type T for the Value{T}.
        /// </summary>
        /// <param name="valueType">The Value{T} to get the type of T for.</param>
        /// <returns>The closing T type if the valueType is a Value{T}, otherwise null.</returns>
        public static Type GetValueType(this Type valueType)
        {
            if (valueType?.IsValue() != true) return null;

            var openValueType = typeof(Value<>);
            var typeToCheck = valueType;
            while (typeToCheck != null && typeToCheck != typeof(object))
            {
                var currentType = typeToCheck.GetTypeInfo().IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck;
                if (openValueType == currentType)
                {
                    return typeToCheck.GetTypeInfo().GenericTypeArguments[0];
                }

                typeToCheck = typeToCheck.GetTypeInfo().BaseType;
            }

            return null;
        }

        /// <summary>
        /// Gets the closing type T for the Value{T}.
        /// </summary>
        /// <param name="instance">The Value{T} instance to get the type of T for.</param>
        /// <returns>The closing T type if the instance is a Value{T} instance, otherwise null.</returns>
        public static Type GetValueType(this object instance)
        {
            return GetValueType(instance?.GetType());
        }
    }
}
