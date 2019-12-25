// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation.Rules
{
    /// <summary>
    /// Represents the <see cref="ValueRule"/> for requiring the value.
    /// </summary>
    public class Required : ValueRule
    {
        /// <summary>
        /// When a value is null, this is the reason given.
        /// </summary>
        public static Reason ValueIsNull = Reason.Create("712D26C6-A40F-4A3D-8C69-1475E761A1CF", "Value is null");

        /// <summary>
        /// When a value is not specified, this is the reason given.
        /// </summary>
        public static Reason StringIsEmpty = Reason.Create("6DE903D6-014C-4B07-B5D3-C3F28677C1A6", "String is empty");

        /// <summary>
        /// When a value is not specified, this is the reason given.
        /// </summary>
        public static Reason ValueNotSpecified = Reason.Create("5F790FC3-5C7D-4F3A-B1E9-8F85FAF7176D", "Valud not specified");

        /// <summary>
        /// Initializes a new instance of the <see cref="Required"/> class.
        /// </summary>
        /// <param name="property"><see cref="PropertyInfo">Property</see> the rule is for.</param>
        public Required(PropertyInfo property)
            : base(property)
        {
        }

        /// <inheritdoc/>
        public override void Evaluate(IRuleContext context, object instance)
        {
            if (instance == null) context.Fail(this, instance, ValueIsNull.NoArgs());
            if (instance is string x && x?.Length == 0) context.Fail(this, instance, StringIsEmpty.NoArgs());

            if (instance != null)
            {
                var type = instance.GetType();
                if (type.HasDefaultConstructor())
                {
                    if (Activator.CreateInstance(type).Equals(instance)) context.Fail(this, instance, ValueNotSpecified.NoArgs());
                }
            }
        }
    }
}
