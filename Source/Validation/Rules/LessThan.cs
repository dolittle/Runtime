// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation.Rules
{
    /// <summary>
    /// Represents the <see cref="ValueRule"/> for less than - any value must be less than a given value.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    public class LessThan<T> : ValueRule
        where T : IComparable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LessThan{T}"/> class.
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo">Property</see> the rule is for.</param>
        /// <param name="value">Value that the input value must be less than.</param>
        public LessThan(PropertyInfo property, T value)
            : base(property)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value that input value must be less than.
        /// </summary>
        public T Value { get; }

        /// <inheritdoc/>
        public override void Evaluate(IRuleContext context, object instance)
        {
            if (FailIfValueTypeMismatch<T>(context, instance))
            {
                var comparison = ((IComparable<T>)instance).CompareTo(Value);
                if (comparison == 0) context.Fail(this, instance, Reasons.ValueIsEqual.WithArgs(new { LeftHand = instance, RightHand = Value }));
                if (comparison > 0) context.Fail(this, instance, Reasons.ValueIsGreaterThan.WithArgs(new { LeftHand = instance, RightHand = Value }));
            }
        }
    }
}
