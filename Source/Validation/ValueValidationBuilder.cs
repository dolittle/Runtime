// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;

namespace Dolittle.Validation
{
    /// <summary>
    /// Defines a builder for building rules for value validation.
    /// </summary>
    /// <typeparam name="TValue">Value type for the builder.</typeparam>
    public class ValueValidationBuilder<TValue> : IValueValidationBuilder
    {
        readonly List<IValueRule> _rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueValidationBuilder{TValue}"/> class.
        /// </summary>
        /// <param name="property">Property that represents the value.</param>
        public ValueValidationBuilder(PropertyInfo property)
        {
            _rules = new List<IValueRule>();
            Property = property;
        }

        /// <inheritdoc/>
        public PropertyInfo Property { get; }

        /// <inheritdoc/>
        public IEnumerable<IValueRule> Rules => _rules;

        /// <inheritdoc/>
        public void AddRule(IValueRule rule)
        {
            _rules.Add(rule);
        }
    }
}
