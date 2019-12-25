// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Validation;

namespace Dolittle.Runtime.Commands.Validation
{
    /// <summary>
    /// Represents the result of validation for a <see cref="CommandRequest"/>.
    /// </summary>
    public class CommandValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidationResult"/> class.
        /// </summary>
        public CommandValidationResult()
        {
            CommandErrorMessages = Array.Empty<string>();
            ValidationResults = Array.Empty<ValidationResult>();
        }

        /// <summary>
        /// Gets or sets the error messages related to a command, typically as a result of a failing ModelRule used from the validator.
        /// </summary>
        public IEnumerable<string> CommandErrorMessages { get; set; }

        /// <summary>
        /// Gets or sets the validation results from any validator.
        /// </summary>
        public IEnumerable<ValidationResult> ValidationResults { get; set; }
    }
}
