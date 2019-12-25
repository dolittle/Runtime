// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Rules;

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Represents the result of query validation, typically done by <see cref="IQueryValidator"/>.
    /// </summary>
    public class QueryValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryValidationResult"/> class.
        /// </summary>
        /// <param name="brokenRules">Broken rules.</param>
        public QueryValidationResult(IEnumerable<BrokenRule> brokenRules)
        {
            BrokenRules = brokenRules ?? Array.Empty<BrokenRule>();
        }

        /// <summary>
        /// Gets all the broken rules.
        /// </summary>
        public IEnumerable<BrokenRule> BrokenRules { get; }

        /// <summary>
        /// Gets a value indicating whether or not the validation was successful.
        /// </summary>
        public bool Success => !BrokenRules.Any();
    }
}
