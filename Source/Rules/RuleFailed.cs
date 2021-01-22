// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Delegate that gets called when a <see cref="IRule"/> fails.
    /// </summary>
    /// <param name="rule"><see cref="IRule"/> that is failing.</param>
    /// <param name="instance">Instance it was evaluating.</param>
    /// <param name="cause"><see cref="Cause"/> for failing.</param>
    public delegate void RuleFailed(IRule rule, object instance, Cause cause);
}
