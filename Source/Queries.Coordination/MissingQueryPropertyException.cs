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
    public class MissingQueryProperty : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MissingQueryProperty"/>
        /// </summary>
        /// <param name="queryType">Type of <see cref="IQuery"/> that does not have the property on it</param>
        public MissingQueryProperty(Type queryType)
            : base(string.Format("No query property for {0}. Hint: It should be a public instance property with a get on it.", queryType.FullName))
        {
        }
    }
}
