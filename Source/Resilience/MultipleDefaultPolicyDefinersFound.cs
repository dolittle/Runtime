// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Exception that gets thrown whenthere are multiple implementations of <see cref="IDefineDefaultPolicy"/> in the system.
    /// </summary>
    public class MultipleDefaultPolicyDefinersFound : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleDefaultPolicyDefinersFound"/> class.
        /// </summary>
        public MultipleDefaultPolicyDefinersFound()
            : base("Multiple implementations of IDefineDefaultPolicy found - there can be only one")
        {
        }
    }
}