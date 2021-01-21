// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Defines the basis for a rule.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Evaluates the given <see cref="IRuleContext"/> to see if the rule is satisfied.
        /// </summary>
        /// <param name="context">The <see cref="IRuleContext"/> to evaluate for.</param>
        /// <param name="instance">The instance to check if satisfies the rule.</param>
        void Evaluate(IRuleContext context, object instance);
    }
}
