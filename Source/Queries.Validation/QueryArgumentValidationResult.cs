// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Rules;

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Represents the result of query argument validation, typically done by <see cref="IQueryValidator"/>.
    /// </summary>
    public class QueryArgumentValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryArgumentValidationResult"/> class.
        /// </summary>
        /// <param name="brokenRules">Broken rules.</param>
        public QueryArgumentValidationResult(IEnumerable<BrokenRule> brokenRules)
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
