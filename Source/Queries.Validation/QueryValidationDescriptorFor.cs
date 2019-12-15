// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dolittle.Collections;
using Dolittle.Reflection;
using Dolittle.Validation;

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Represents the basis for a validation descriptor for describing validation for queries.
    /// </summary>
    /// <typeparam name="TQuery">Type of <see cref="IQuery"/> descriptor is for.</typeparam>
    public class QueryValidationDescriptorFor<TQuery> : IQueryValidationDescriptor
        where TQuery : IQuery
    {
        readonly Dictionary<string, IValueValidationBuilder> _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryValidationDescriptorFor{TQuery}"/> class.
        /// </summary>
        public QueryValidationDescriptorFor()
        {
            _arguments = new Dictionary<string, IValueValidationBuilder>();
        }

        /// <summary>
        /// Gets the <see cref="IValueValidationBuilder">rule builders</see> for the <see cref="IQuery">query </see>arguments.
        /// </summary>
        public IEnumerable<IValueValidationBuilder> ArgumentsRuleBuilders => _arguments.Values;

        /// <inheritdoc/>
        public IEnumerable<IValueRule> ArgumentRules
        {
            get
            {
                var rules = new List<IValueRule>();
                _arguments.Values.ForEach(r => rules.AddRange(r.Rules));
                return rules;
            }
        }

        /// <summary>
        /// Start describing an argument on a <see cref="IQuery"/>.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="expression">Expression pointing to the argument on the query.</param>
        /// <returns>A <see cref="QueryArgumentValidationBuilder{TQ, TA}"/> for building the rules for the argument.</returns>
        public QueryArgumentValidationBuilder<TQuery, TArgument> ForArgument<TArgument>(Expression<Func<TQuery, TArgument>> expression)
        {
            var property = expression.GetPropertyInfo();
            var builder = new QueryArgumentValidationBuilder<TQuery, TArgument>(property);
            _arguments[property.Name] = builder;
            return builder;
        }
    }
}
