/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Rules;

namespace Dolittle.Runtime.Commands
{
    /// <summary>
    /// Represents the result of a <see cref="BrokenRule"/>
    /// </summary>
    public class BrokenRuleResult
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BrokenRuleResult"/>
        /// </summary>
        /// <param name="rule">Name of rule that was broken</param>
        /// <param name="target">Identifying target that the rule is broken for</param>
        /// <param name="instance">String representation of the instance that was involved in the rule being broken</param>
        /// <param name="causes">All the <see cref="Cause">causes</see></param>
        public BrokenRuleResult(
            string rule,
            string target,
            string instance,
            IEnumerable<Cause> causes)
        {
            Rule = rule;
            Target = target;
            Instance = instance;
            Causes = causes;
        }

        /// <summary>
        /// Gets the name of the rule that was broken
        /// </summary>
        public string Rule { get; }

        /// <summary>
        /// Get the target the broken rule is for
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Gets the string representation of the instance that was involved in the rule being broken
        /// </summary>
        public string Instance { get; }

        /// <summary>
        /// Gets all the causes for the rule being broken
        /// </summary>
        public IEnumerable<Cause> Causes { get; }
    }
}