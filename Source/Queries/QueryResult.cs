// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Rules;

namespace Dolittle.Queries
{
    /// <summary>
    /// Represents the result of a query.
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// Gets or sets the name of the query.
        /// </summary>
        public string QueryName { get; set; }

        /// <summary>
        /// Gets or sets the count of total items from a query.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the items as the result of a query.
        /// </summary>
        public IEnumerable Items { get; set; }

        /// <summary>
        /// Gets or sets the exception that occured during execution.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the messages that are related to broken security rules.
        /// </summary>
        public IEnumerable<string> SecurityMessages { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets all the broken rules.
        /// </summary>
        public IEnumerable<BrokenRule> BrokenRules { get; set; } = Array.Empty<BrokenRule>();

        /// <summary>
        /// Gets a value indicating whether or not query passed security.
        /// </summary>
        public bool PassedSecurity => SecurityMessages?.Any() == false;

        /// <summary>
        /// Gets a value indicating whether or not the query was successful.
        /// </summary>
        public bool Success =>
            Exception == null &&
            Items != null &&
            !Invalid &&
            PassedSecurity;

        /// <summary>
        /// Gets a value indicating whether or not the query is considered invalid in validation terms.
        /// </summary>
        public bool Invalid => BrokenRules.Any();

        /// <summary>
        /// Creates a <see cref="QueryResult"/> for a given <see cref="IQuery"/>.
        /// </summary>
        /// <param name="query"><see cref="IQuery"/> to create for.</param>
        /// <returns><see cref="QueryResult"/> for the given <see cref="IQuery"/>.</returns>
        public static QueryResult For(IQuery query)
        {
            return new QueryResult
            {
                QueryName = query.GetType().Name
            };
        }
    }
}