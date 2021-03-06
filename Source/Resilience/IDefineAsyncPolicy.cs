// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Defines a system that is capable of defining an async policy.
    /// </summary>
    public interface IDefineAsyncPolicy
    {
        /// <summary>
        /// Define the policy.
        /// </summary>
        /// <returns>The defined <see cref="Polly.IAsyncPolicy"/>.</returns>
        Polly.IAsyncPolicy Define();
    }
}