// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Validation
{
    /// <summary>
    /// Defines the behavior of being able to do validation.
    /// </summary>
    public interface ICanValidate
    {
        /// <summary>
        /// Validates that the object is in a valid state.
        /// </summary>
        /// <param name="target">The target to validate.</param>
        /// <returns>A collection of ValidationResults.  An empty collection indicates a valid command.</returns>
        IEnumerable<ValidationResult> ValidateFor(object target);
    }

    /// <summary>
    /// Defines the behavior of being able to do validation.
    /// </summary>
    /// <typeparam name="T">Type it can validate.</typeparam>
    public interface ICanValidate<in T> : ICanValidate
    {
        /// <summary>
        /// Validates that the object is in a valid state.
        /// </summary>
        /// <param name="target">The target to validate.</param>
        /// <returns>A collection of ValidationResults.  An empty collection indicates a valid command.</returns>
        IEnumerable<ValidationResult> ValidateFor(T target);
    }
}