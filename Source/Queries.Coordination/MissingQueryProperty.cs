// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Queries.Coordination
{
    /// <summary>
    /// Exception that gets thrown when a well known query does not have the query property on it.
    /// </summary>
    public class MissingQueryProperty : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingQueryProperty"/> class.
        /// </summary>
        /// <param name="queryType">Type of <see cref="IQuery"/> that does not have the property on it.</param>
        public MissingQueryProperty(Type queryType)
            : base($"No query property for {queryType.FullName}. Hint: It should be a public instance property with a get on it.")
        {
        }
    }
}
