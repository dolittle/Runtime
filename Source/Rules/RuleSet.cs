// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents a set of <see cref="IRule">rules</see>.
    /// </summary>
    public class RuleSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleSet"/> class.
        /// </summary>
        /// <param name="rules"><see cref="IEnumerable{T}"/> of <see cref="IRule">rules</see>.</param>
        public RuleSet(IEnumerable<IRule> rules)
        {
            Rules = rules;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleSet"/> class.
        /// </summary>
        /// <param name="rules">Params of <see cref="IRule">rules</see>.</param>
        public RuleSet(params IRule[] rules)
        {
            Rules = rules;
        }

        /// <summary>
        /// Gets the <see cref="IRule">rules</see> in the <see cref="RuleSet"/>.
        /// </summary>
        public IEnumerable<IRule> Rules { get; }
    }
}
