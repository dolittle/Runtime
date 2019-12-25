// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation.Rules
{
    /// <summary>
    /// Represents the <see cref="ValueRule"/> for specific length - any value must be a specific length.
    /// </summary>
    public class MaxLength : ValueRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLength"/> class.
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo">Property</see> the rule is for.</param>
        /// <param name="length">The required length.</param>
        public MaxLength(PropertyInfo property, int length)
            : base(property)
        {
            Length = length;
        }

        /// <summary>
        /// Gets the required length.
        /// </summary>
        public int Length { get; }

        /// <inheritdoc/>
        public override void Evaluate(IRuleContext context, object instance)
        {
            if (FailIfValueTypeMismatch<string>(context, instance))
            {
                var length = ((string)instance).Length;
                if (length > Length) context.Fail(this, instance, Reasons.LengthIsTooLong.WithArgs(new { Length = length }));
            }
        }
    }
}
