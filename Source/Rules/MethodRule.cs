// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Collections;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents a <see cref="IRule"/> that is implemented as a method.
    /// </summary>
    public class MethodRule : IRule
    {
        readonly Func<RuleEvaluationResult> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodRule"/> class.
        /// </summary>
        /// <param name="name">Name of method.</param>
        /// <param name="method"><see cref="Func{T}"/> representing the method.</param>
        public MethodRule(string name, Func<RuleEvaluationResult> method)
        {
            Name = name;
            _method = method;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public void Evaluate(IRuleContext context, object instance)
        {
            var result = _method();
            if (!result.IsSuccess)
            {
                result.Causes.ForEach(_ => context.Fail(this, instance, _));
            }
        }
    }
}
