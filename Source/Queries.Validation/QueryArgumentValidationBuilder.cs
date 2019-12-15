// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Validation;

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Represents a builder for building validation description for a query.
    /// </summary>
    /// <typeparam name="TQuery">Type of query.</typeparam>
    /// <typeparam name="TArgument">Type of argument.</typeparam>
    public class QueryArgumentValidationBuilder<TQuery, TArgument> : ValueValidationBuilder<TArgument>
        where TQuery : IQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryArgumentValidationBuilder{TQuery, TArgument}"/> class.
        /// </summary>
        /// <param name="property">Property that represents the argument.</param>
        public QueryArgumentValidationBuilder(PropertyInfo property)
            : base(property)
        {
        }
    }
}
