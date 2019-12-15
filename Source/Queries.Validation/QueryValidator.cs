// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Collections;
using Dolittle.Reflection;
using Dolittle.Rules;

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Represents an implementation of <see cref="IQueryValidator"/>.
    /// </summary>
    public class QueryValidator : IQueryValidator
    {
        readonly IQueryValidationDescriptors _descriptors;
        readonly IRuleContexts _ruleContexts;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryValidator"/> class.
        /// </summary>
        /// <param name="descriptors"><see cref="IQueryValidationDescriptors"/> for getting descriptors for queries for running through rules.</param>
        /// <param name="ruleContexts"><see cref="IRuleContexts"/> used for getting <see cref="IRuleContext"/>.</param>
        public QueryValidator(IQueryValidationDescriptors descriptors, IRuleContexts ruleContexts)
        {
            _descriptors = descriptors;
            _ruleContexts = ruleContexts;
        }

        /// <inheritdoc/>
        public QueryValidationResult Validate(IQuery query)
        {
            var brokenRules = new Dictionary<IRule, BrokenRule>();

            var ruleContext = _ruleContexts.GetFor(query);
            ruleContext.OnFailed(RuleFailed(ruleContext, brokenRules));

            var hasDescriptor = _descriptors.CallGenericMethod<bool, IQueryValidationDescriptors>(d => d.HasDescriptorFor<IQuery>, query.GetType());
            if (hasDescriptor)
            {
                var descriptor = _descriptors.CallGenericMethod<IQueryValidationDescriptor, IQueryValidationDescriptors>(d => d.GetDescriptorFor<IQuery>, query.GetType());
                descriptor.ArgumentRules.ForEach(r =>
                {
                    var value = r.Property.GetValue(query);
                    r.Evaluate(ruleContext, value);
                });
            }

            var result = new QueryValidationResult(brokenRules.Values);
            return result;
        }

        RuleFailed RuleFailed(IRuleContext ruleContext, Dictionary<IRule, BrokenRule> brokenRules)
        {
            return (rule, instance, cause) =>
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

                brokenRule.AddCause(cause);
            };
        }
    }
}
