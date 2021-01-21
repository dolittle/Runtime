// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Collections;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents an evaluation of a <see cref="RuleSet"/>.
    /// </summary>
    public class RuleSetEvaluation
    {
        readonly RuleSet _ruleSet;
        readonly List<BrokenRule> _brokenRules = new List<BrokenRule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleSetEvaluation"/> class.
        /// </summary>
        /// <param name="ruleSet"><see cref="RuleSet"/> to evaluate.</param>
        public RuleSetEvaluation(RuleSet ruleSet)
        {
            _ruleSet = ruleSet;
        }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="RuleSetEvaluation"/> was successful.
        /// </summary>
        public bool IsSuccess => _brokenRules.Count == 0;

        /// <summary>
        /// Gets the <see cref="BrokenRule">broken rules</see>.
        /// </summary>
        public IEnumerable<BrokenRule> BrokenRules => _brokenRules;

        /// <summary>
        /// Implicitly convert to <see cref="bool"/> for evaluating the success of the evaluation.
        /// </summary>
        /// <param name="input"><see cref="RuleSetEvaluation"/> to convert from.</param>
        public static implicit operator bool(RuleSetEvaluation input) => input.IsSuccess;

        /// <summary>
        /// Evaluate against a target.
        /// </summary>
        /// <param name="target">Target to evaluate.</param>
        public void Evaluate(object target)
        {
            var brokenRules = new Dictionary<IRule, BrokenRule>();

            var context = new RuleContext(target);
            context.OnFailed(RuleFailed(context, brokenRules));

            _ruleSet.Rules.ForEach(_ => _.Evaluate(context, target));

            _brokenRules.AddRange(brokenRules.Values);
        }

        RuleFailed RuleFailed(IRuleContext ruleContext, Dictionary<IRule, BrokenRule> brokenRules)
        {
            return (rule, instance, reason) =>
            {
                BrokenRule brokenRule;
                if (brokenRules.ContainsKey(rule))
                {
                    brokenRule = brokenRules[rule];
                }
                else
                {
                    brokenRule = new BrokenRule(rule, instance, ruleContext);
                    brokenRules[rule] = brokenRule;
                }

                brokenRule.AddCause(reason);
            };
        }
    }
}
