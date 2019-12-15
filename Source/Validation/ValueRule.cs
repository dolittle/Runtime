// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation
{
    /// <summary>
    /// Represents the basis for a value rule.
    /// </summary>
    public abstract class ValueRule : IValueRule
    {
        /// <summary>
        /// When a value is of the wrong type, this is the reason given for breaking a rule.
        /// </summary>
        public static Reason ValueTypeMismatch = Reason.Create("150757B0-8118-42FB-A8C4-2D49E7AC3AFD", "Value type mismatch - expected {Expected} got {Type}");

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueRule"/> class.
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo">Property</see> the rule is for.</param>
        protected ValueRule(PropertyInfo property)
        {
            Name = GetType().Name;
            Property = property;
        }

        /// <inheritdoc/>
        public virtual string Name { get; }

        /// <inheritdoc/>
        public PropertyInfo Property { get; }

        /// <inheritdoc/>
        public abstract void Evaluate(IRuleContext context, object instance);

        /// <summary>
        /// Method to call if value type mismatch - produces a failture in the <see cref="IRuleContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="IRuleContext"/>.</param>
        /// <param name="value">The value to check.</param>
        /// <typeparam name="TDesired">Desired type.</typeparam>
        /// <returns>true if its ok, false if it failed.</returns>
        protected bool FailIfValueTypeMismatch<TDesired>(IRuleContext context, object value)
        {
            if (value.GetType() != typeof(TDesired))
            {
                context.Fail(this, value, ValueTypeMismatch.WithArgs(new { Expected = typeof(TDesired), Types = value.GetType() }));
                return false;
            }

            return true;
        }
    }
}
