// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the result of the validation of a <see cref="IFilterDefinition" />.
    /// </summary>
    public class FilterValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidationResult"/> class.
        /// </summary>
        public FilterValidationResult() => Succeeded = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidationResult"/> class.
        /// </summary>
        /// <param name="reason">The <see cref="FailedFilterValidationReason" />.</param>
        public FilterValidationResult(FailedFilterValidationReason reason) => FailureReason = reason;

        /// <summary>
        /// Gets a value indicating whether the validation succeeded or not.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the <see cref="FailedFilterValidationReason" />.
        /// </summary>
        public FailedFilterValidationReason FailureReason { get; }
    }
}
