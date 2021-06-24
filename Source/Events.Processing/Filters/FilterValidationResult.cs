// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

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
        FilterValidationResult() => Success = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterValidationResult"/> class.
        /// </summary>
        /// <param name="reason">The <see cref="FailedFilterValidationReason" />.</param>
        FilterValidationResult(FailedFilterValidationReason reason) => FailureReason = reason;

        /// <summary>
        /// Gets a value indicating whether the validation succeeded or not.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the <see cref="FailedFilterValidationReason" />.
        /// </summary>
        public FailedFilterValidationReason FailureReason { get; }

        /// <summary>
        /// Creates a failed <see cref="FilterValidationResult" />.
        /// </summary>
        /// <param name="reason">The <see cref="FailedFilterValidationReason" />.</param>
        /// <returns>The <see cref="FilterValidationResult" />.</returns>
        public static FilterValidationResult Failed(FailedFilterValidationReason reason) => new(reason);

        /// <summary>
        /// Creates a succeeded <see cref="FilterValidationResult" />.
        /// </summary>
        /// <returns>The <see cref="FilterValidationResult" />.</returns>
        public static FilterValidationResult Succeeded() => new();
    }
}
