// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation.Rules
{
    /// <summary>
    /// Represents the <see cref="ValueRule"/> for greater than or equal - any value must be greater than or equal to a given value.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    public class GreaterThanOrEqual<T> : ValueRule
        where T : IComparable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanOrEqual{T}"/> class.
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo">Property</see> the rule is for.</param>
        /// <param name="value">Value that the input value must be greater than or equal.</param>
        public GreaterThanOrEqual(PropertyInfo property, T value)
            : base(property)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value that input value must be greater than or equal.
        /// </summary>
        public T Value { get; }

        /// <inheritdoc/>
        public override void Evaluate(IRuleContext context, object instance)
        {
            if (FailIfValueTypeMismatch<T>(context, instance))
            {
                var comparison = ((IComparable<T>)instance).CompareTo(Value);
                if (comparison < 0) context.Fail(this, instance, Reasons.ValueIsLessThan.WithArgs(new { LeftHand = instance, RightHand = Value }));
            }
        }
    }
}
