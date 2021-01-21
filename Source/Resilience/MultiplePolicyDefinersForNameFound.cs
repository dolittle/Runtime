// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Exception that gets thrown whenthere are multiple implementations of <see cref="IDefineNamedPolicy"/> in the system.
    /// </summary>
    public class MultiplePolicyDefinersForNameFound : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplePolicyDefinersForNameFound"/> class.
        /// </summary>
        /// <param name="name">Name of policy not found.</param>
        public MultiplePolicyDefinersForNameFound(string name)
            : base($"Multiple implementations of IDefineNamedPolicy found for '{name}' - there can be only one")
        {
        }
    }
}