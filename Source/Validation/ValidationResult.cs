// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Validation
{
    /// <summary>
    /// Represents a container for the results of a validation request.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errorMessage">Error message for the validation result.</param>
        public ValidationResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errorMessage">Error message for the validation result.</param>
        /// <param name="memberNames">The members for the validation result.</param>
        public ValidationResult(string errorMessage, IEnumerable<string> memberNames)
        {
            ErrorMessage = errorMessage;
            MemberNames = memberNames;
        }

        /// <summary>
        /// Gets the error message for the validation.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the collection of member names that indicate which fields have validation.
        /// </summary>
        /// <returns>The collection of member names that indicate which fields have validation errors.</returns>
        public IEnumerable<string> MemberNames { get; }
    }
}