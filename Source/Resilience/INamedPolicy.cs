// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Defines a named policy.
    /// </summary>
    public interface INamedPolicy : IPolicy
    {
        /// <summary>
        /// Gets the name of the policy.
        /// </summary>
        string Name { get; }
    }
}