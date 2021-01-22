// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents the result of a an evaluation of a <see cref="IRule"/>.
    /// </summary>
    public class RuleEvaluationResult
    {
        /// <summary>
        /// Gets the value representing a successful evaluation.
        /// </summary>
        public static readonly RuleEvaluationResult Success = new RuleEvaluationResult(string.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleEvaluationResult"/> class.
        /// </summary>
        /// <param name="instance">Instance that was evaluated.</param>
        /// <param name="causes">Params of <see cref="Cause">causes</see>.</param>
        public RuleEvaluationResult(object instance, params Cause[] causes)
        {
            Instance = instance;
            Causes = causes;
        }

        /// <summary>
        /// Gets the instance that was evaluated.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// Gets the <see cref="Cause">causes</see> - if there were anything broken.
        /// </summary>
        public IEnumerable<Cause> Causes { get; }

        /// <summary>
        /// Gets a value indicating whether or not the result is successful.
        /// </summary>
        public bool IsSuccess => !Causes.Any();

        /// <summary>
        /// Implicitly convert from <see cref="RuleEvaluationResult"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="result"><see cref="RuleEvaluationResult"/> to convert from.</param>
        public static implicit operator bool(RuleEvaluationResult result) => result.IsSuccess;

        /// <summary>
        /// Create a failed <see cref="RuleEvaluationResult"/> with any <see cref="Cause">causes</see>.
        /// </summary>
        /// <param name="instance">Instance to fail.</param>
        /// <param name="causes">Params of <see cref="Cause">causes</see> to fail.</param>
        /// <returns><see cref="RuleEvaluationResult"/> with the <see cref="Cause">causes</see>.</returns>
        public static RuleEvaluationResult Fail(object instance, params Cause[] causes)
        {
            return new RuleEvaluationResult(instance, causes);
        }
    }
}
