// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Dolittle.Runtime.Concepts
{
    /// <summary>
    /// Maps a concept type to the underlying primitive type.
    /// </summary>
    public static class ConceptMap
    {
        static readonly ConcurrentDictionary<Type, Type> _cache = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// Get the type of the value in a <see cref="ConceptAs{T}"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to get value type from.</param>
        /// <returns>The type of the <see cref="ConceptAs{T}"/> value.</returns>
        public static Type GetConceptValueType(Type type)
        {
            return _cache.GetOrAdd(type, GetPrimitiveType);
        }

        static Type GetPrimitiveType(Type type)
        {
            var conceptType = type;
            for (; ;)
            {
                if (conceptType == typeof(ConceptAs<>)) break;

                var typeProperty = conceptType.GetTypeInfo().GetProperty("UnderlyingType");
                if (typeProperty != null)
                {
                    return (Type)typeProperty.GetValue(null);
                }

                if (conceptType == typeof(object)) break;

                conceptType = conceptType.GetTypeInfo().BaseType;
            }

            return null;
        }
    }
}