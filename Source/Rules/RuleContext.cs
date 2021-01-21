// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents an implementation of <see cref="IRuleContext"/>.
    /// </summary>
    public class RuleContext : IRuleContext
    {
        readonly List<RuleFailed> _callbacks = new List<RuleFailed>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleContext"/> class.
        /// </summary>
        /// <param name="target">Target the <see cref="RuleContext"/> is for.</param>
        public RuleContext(object target)
        {
            Target = target;
        }

        /// <inheritdoc/>
        public object Target { get; }

        /// <inheritdoc/>
        public void OnFailed(RuleFailed callback)
        {
            _callbacks.Add(callback);
        }

        /// <inheritdoc/>
        public void Fail(IRule rule, object instance, Cause cause)
        {
            _callbacks.ForEach(c => c(rule, instance, cause));
        }
    }
}
