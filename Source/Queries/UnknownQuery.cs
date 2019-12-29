// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Queries
{
    /// <summary>
    /// Exception that gets thrown when a query is not known by its name in the system.
    /// </summary>
    public class UnknownQuery : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownQuery"/> class.
        /// </summary>
        /// <param name="name">Name of query.</param>
        public UnknownQuery(string name)
            : base("There is no query named : " + name)
        {
        }
    }
}