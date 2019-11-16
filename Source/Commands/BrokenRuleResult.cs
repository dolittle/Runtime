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
        /// <param name="reasons">All the <see cref="BrokenRuleReason">reasons</see></param>
        public BrokenRuleResult(string rule, string target, IEnumerable<BrokenRuleReason> reasons)
        {
            Rule = rule;
            Target = target;
            Reasons = reasons;
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
        /// Gets all the reasons for the rule being broken
        /// </summary>
        public IEnumerable<BrokenRuleReason> Reasons { get; }
    }
}