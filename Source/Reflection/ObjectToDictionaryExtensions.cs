// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#pragma warning disable DL0008

namespace Dolittle.Runtime.Reflection
{
    /// <summary>
    /// Provides extension methods for converting any <see cref="object"/> to a <see cref="IDictionary"/>.
    /// </summary>
    public static class ObjectToDictionaryExtensions
    {
        /// <summary>
        /// Convert an <see cref="object"/> to a <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="source"><see cref="object"/> to turn into a dictionary.</param>
        /// <returns><see cref="IDictionary"/> with all keys and values.</returns>
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        /// <summary>
        /// Convert an <see cref="object"/> to a <see cref="IDictionary"/>.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="source"><see cref="object"/> to turn into a dictionary.</param>
        /// <returns><see cref="IDictionary"/> with all keys and values.</returns>
        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            ThrowIfSourceArgumentIsNull(source);

            var dictionary = new Dictionary<string, T>();
            foreach (var property in source.GetType().GetTypeInfo().GetProperties())
                AddPropertyToDictionary<T>(property, source, dictionary);

            return dictionary;
        }

        static void AddPropertyToDictionary<T>(PropertyInfo property, object source, Dictionary<string, T> dictionary)
        {
            var value = property.GetValue(source);
            if (IsOfType<T>(value))
                dictionary.Add(property.Name, (T)value);
        }

        static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        static void ThrowIfSourceArgumentIsNull(object source)
        {
            if (source == null) throw new SourceObjectCannotBeNull();
        }
    }
}
