/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation.Rules
{
    /// <summary>
    /// Represents the <see cref="ValueRule"/> for requiring the value to not be null
    /// </summary>
    public class NotNull : ValueRule
    {
        /// <summary>
        /// When a value is null, this is the reason given 
        /// </summary>
        public static Reason ValueIsNull = Reason.Create("712D26C6-A40F-4A3D-8C69-1475E761A1CF", "Value is null");

        /// <summary>
        /// Initializes a new instance of the <see cref="NotNull"/> rule
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo">Property</see> the rule is for</param>
        public NotNull(PropertyInfo property) : base(property) { }

        /// <inheritdoc/>
        public override void Evaluate(IRuleContext context, object instance)
        {
            if (instance == null) context.Fail(this, instance, ValueIsNull.NoArgs());
        }
    }
}
