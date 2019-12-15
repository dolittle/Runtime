// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.ReadModels
{
    /// <summary>
    /// The exception that is thrown when a readmodelof is not known by its name in the system.
    /// </summary>
    public class UnknownReadModelOfException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownReadModelOfException"/> class.
        /// </summary>
        /// <param name="name">Name of missing read model type.</param>
        public UnknownReadModelOfException(string name)
            : base("There is no readmodelof named : " + name)
        {
        }
    }
}