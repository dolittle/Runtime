/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation
{
    /// <summary>
    /// Represents the basis for a value rule
    /// </summary>  
    public abstract class ValueRule : IValueRule
    {
        /// <summary>
        /// When a value is of the wrong type, this is the reason given for breaking a rule
        /// </summary>
        public static BrokenRuleReason ValueTypeMismatch = BrokenRuleReason.Create("150757B0-8118-42FB-A8C4-2D49E7AC3AFD", "Value type mismatch");

        /// <summary>
        /// Initializes a new instance of <see cref="ValueRule"/>
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo">Property</see> the rule is for</param>
        protected ValueRule(PropertyInfo property)
        {
            Property = property;
        }

        /// <inheritdoc/>
        public PropertyInfo Property { get; }

        /// <inheritdoc/>
        protected bool FailIfValueTypeMismatch<TDesired>(IRuleContext context, object value)
        {
            if (value.GetType() != typeof(TDesired))
            {
                context.Fail(this, value, ValueTypeMismatch);
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public abstract void Evaluate(IRuleContext context, object instance);
    }
}
