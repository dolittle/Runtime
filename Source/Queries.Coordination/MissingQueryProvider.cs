/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace Dolittle.Queries.Coordination
{
    /// <summary>
    /// The exception that is thrown when a well known query does not have the query property on it
    /// </summary>
    public class MissingQueryProvider : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MissingQueryProvider"/>
        /// </summary>
        /// <param name="queryType">Type of <see cref="IQuery"/> that we can't find a <see cref="IQueryProviderFor{T}"/></param>
        /// <param name="type"><see cref="Type"/> of the expected query returned from the Query property</param>
        public MissingQueryProvider(Type queryType, Type type)
            : base(string.Format("Unable to find a query provider of type '{0}' for the query '{1}'. Hint: Are you sure the query return type has a known query provider for it?", type.FullName, queryType.GetType().FullName))
        {
        }
    }
}
