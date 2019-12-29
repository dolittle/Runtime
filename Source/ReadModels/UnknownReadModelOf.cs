// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.ReadModels
{
    /// <summary>
    /// Exception that gets thrown when a readmodelof is not known by its name in the system.
    /// </summary>
    public class UnknownReadModelOf : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownReadModelOf"/> class.
        /// </summary>
        /// <param name="name">Name of missing read model type.</param>
        public UnknownReadModelOf(string name)
            : base("There is no readmodelof named : " + name)
        {
        }
    }
}