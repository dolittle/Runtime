// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Rules;

namespace Dolittle.Validation
{
    /// <summary>
    /// Defines the basis for a builder for value validation.
    /// </summary>
    public interface IValueValidationBuilder : IRuleBuilder<IValueRule>
    {
        /// <summary>
        /// Gets the property that will be used in any of the rules in this <see cref="IRuleBuilder{T}">builder</see>.
        /// </summary>
        PropertyInfo Property { get; }
    }
}
