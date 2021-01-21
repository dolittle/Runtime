// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Defines the context a rule can evaluate.
    /// </summary>
    public interface IRuleContext
    {
        /// <summary>
        /// Gets the target for the <see cref="RuleContext"/>.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Register callback that gets called if there is a <see cref="IRule">rule</see> that fails.
        /// </summary>
        /// <param name="callback"><see cref="RuleFailed"/> callback.</param>
        void OnFailed(RuleFailed callback);

        /// <summary>
        /// Report a rule as failing.
        /// </summary>
        /// <param name="rule"><see cref="IRule"/> to report.</param>
        /// <param name="instance">The instance that was part of causing the problem.</param>
        /// <param name="cause"><see cref="Cause"/> for it failing.</param>
        void Fail(IRule rule, object instance, Cause cause);
    }
}
