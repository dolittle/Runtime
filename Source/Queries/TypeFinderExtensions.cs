/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Execution;
using Dolittle.Reflection;
using Dolittle.Types;

namespace Dolittle.Queries
{
    /// <summary>
    /// Extension methods for <see cref="ITypeFinder"/> for dealing with ReadModels and Queries
    /// </summary>
    public static class TypeFinderExtensions
    {
        /// <summary>
        /// Get the type of the query matching the fullname.  This can be in any loaded assembly and does not require the assmebly qualified name.
        /// </summary>
        /// <param name="typeFinder">instance of <see cref="ITypeFinder"/> being extended</param>
        /// <param name="fullName">The full name of the type</param>
        /// <returns>the type if found, <see cref="UnknownQueryException" /> if not found or type is not a query</returns>
        public static Type GetQueryTypeByName(this ITypeFinder typeFinder, string fullName)
        {
            var queryType = typeFinder.FindTypeByFullName(fullName);

            if (queryType == null || !queryType.HasInterface(typeof(IQuery)))
                throw new UnknownQueryException(fullName);

            return queryType;
        }
    }
}