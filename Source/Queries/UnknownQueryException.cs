// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Queries
{
    /// <summary>
    /// The exception that is thrown when a query is not known by its name in the system.
    /// </summary>
    public class UnknownQueryException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownQueryException"/> class.
        /// </summary>
        /// <param name="name">Name of query.</param>
        public UnknownQueryException(string name)
            : base("There is no query named : " + name)
        {
        }
    }
}