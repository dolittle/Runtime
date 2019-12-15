// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Defines the system that validates a query.
    /// </summary>
    public interface IQueryValidator
    {
        /// <summary>
        /// Validate a query instance.
        /// </summary>
        /// <param name="query"><see cref="IQuery"/> to validate.</param>
        /// <returns>The <see cref="QueryValidationResult">result</see> of the query.</returns>
        QueryValidationResult Validate(IQuery query);
    }
}
