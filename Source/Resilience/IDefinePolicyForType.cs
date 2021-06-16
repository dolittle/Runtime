// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Defines a system that is capable of defining a resilience policy for a specific type.
    /// </summary>
    public interface IDefinePolicyForType : IDefinePolicy
    {
        /// <summary>
        /// Gets the type it can define for.
        /// </summary>
        Type Type { get; }
    }
}