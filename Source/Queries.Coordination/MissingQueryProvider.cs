// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Queries.Coordination
{
    /// <summary>
    /// Exception that gets thrown when a well known query does not have the query property on it.
    /// </summary>
    public class MissingQueryProvider : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingQueryProvider"/> class.
        /// </summary>
        /// <param name="queryType">Type of <see cref="IQuery"/> that we can't find a <see cref="IQueryProviderFor{T}"/>.</param>
        /// <param name="type"><see cref="Type"/> of the expected query returned from the Query property.</param>
        public MissingQueryProvider(Type queryType, Type type)
            : base($"Unable to find a query provider of type '{type.FullName}' for the query '{queryType.GetType().FullName}'. Hint: Are you sure the query return type has a known query provider for it?")
        {
        }
    }
}
