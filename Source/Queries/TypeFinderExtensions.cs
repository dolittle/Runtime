// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Reflection;
using Dolittle.Types;

namespace Dolittle.Queries
{
    /// <summary>
    /// Extension methods for <see cref="ITypeFinder"/> for dealing with ReadModels and Queries.
    /// </summary>
    public static class TypeFinderExtensions
    {
        /// <summary>
        /// Get the type of the query matching the fullname.  This can be in any loaded assembly and does not require the assmebly qualified name.
        /// </summary>
        /// <param name="typeFinder">instance of <see cref="ITypeFinder"/> being extended.</param>
        /// <param name="fullName">The full name of the type.</param>
        /// <returns>the type if found, <see cref="UnknownQuery" /> if not found or type is not a query.</returns>
        public static Type GetQueryTypeByName(this ITypeFinder typeFinder, string fullName)
        {
            var queryType = typeFinder.FindTypeByFullName(fullName);

            if (queryType?.HasInterface(typeof(IQuery)) != true)
                throw new UnknownQuery(fullName);

            return queryType;
        }
    }
}