// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Represents an argument on a query.
    /// </summary>
    public class QueryArgument
    {
        /// <summary>
        /// Gets the property info for the argument.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets or sets the rules for the argument.
        /// </summary>
        public IEnumerable<IRule> Rules { get; set; }

        /// <summary>
        /// Validate the argument.
        /// </summary>
        /// <param name="context"><see cref="IRuleContext"/> to validate.</param>
        /// <returns>The <see cref="QueryArgumentValidationResult"/>.</returns>
        public QueryArgumentValidationResult Validate(IRuleContext context)
        {
            if (context == null)
            {
                return new QueryArgumentValidationResult(Array.Empty<BrokenRule>());
            }

            var result = new QueryArgumentValidationResult(null);
            return result;
        }
    }
}
