// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Dolittle.Dynamic
{
    /// <summary>
    /// Provides a set of extension methods for working with dynamic types.
    /// </summary>
    public static class DynamicExtensions
    {
        /// <summary>
        /// Converts an object to a dynamic <see cref="ExpandoObject"/>.
        /// </summary>
        /// <param name="source">Source as any <see cref="object"/>.</param>
        /// <returns>Dynamic <see cref="ExpandoObject">expando object</see>.</returns>
        public static dynamic AsExpandoObject(this object source)
        {
            var expando = new ExpandoObject();

            foreach (var property in source.GetType().GetTypeInfo().GetProperties())
                ((IDictionary<string, object>)expando)[property.Name] = property.GetValue(source, null);

            return expando;
        }
    }
}
