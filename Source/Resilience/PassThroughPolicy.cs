// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Resilience
{
    /// <summary>
    /// Represents a null implementation of <see cref="IPolicy"/>.
    /// </summary>
    /// <remarks>
    /// This policy does nothing, just passes through the calls.
    /// If no default policy is defined, this is the policy that typically will be used as the
    /// default policy.
    /// </remarks>
    public class PassThroughPolicy : IPolicy
    {
        /// <inheritdoc/>
        public void Execute(Action action)
        {
            action();
        }

        /// <inheritdoc/>
        public TResult Execute<TResult>(Func<TResult> action)
        {
            return action();
        }
    }
}