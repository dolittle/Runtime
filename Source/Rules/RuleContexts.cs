// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents an implementation of <see cref="IRuleContexts"/>.
    /// </summary>
    public class RuleContexts : IRuleContexts
    {
        /// <inheritdoc/>
        public IRuleContext GetFor(object instance)
        {
            var ruleContext = new RuleContext(instance);
            return ruleContext;
        }
    }
}
