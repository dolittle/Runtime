// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the registration result of the filter <see cref="StreamProcessor "/> for a filter.
    /// </summary>
    /// <typeparam name="TFilterDefinition">The <see cref="IFilterDefinition" />.</typeparam>
    public class FilterRegistrationResult<TFilterDefinition>
        where TFilterDefinition : IFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegistrationResult{T}"/> class.
        /// </summary>
        /// <param name="filterProcessor">The <see cref="IFilterProcessor{T}" />.</param>
        public FilterRegistrationResult(IFilterProcessor<TFilterDefinition> filterProcessor)
        {
            Succeeded = true;
            FilterProcessor = filterProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegistrationResult{T}"/> class.
        /// </summary>
        /// <param name="failureReason">The <see cref="FailedFilterRegistrationReason" />.</param>
        public FilterRegistrationResult(FailedFilterRegistrationReason failureReason)
        {
            Succeeded = false;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets a value indicating whether the registration of the <see cref="StreamProcessor">stream processors</see> for the event handler succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the <see cref="IFilterProcessor{T}" /> for TFilterDefinition.
        /// </summary>
        public IFilterProcessor<TFilterDefinition> FilterProcessor { get; }

        /// <summary>
        /// Gets the reason for why the registration failed.
        /// </summary>
        public FailedFilterRegistrationReason FailureReason { get; }
    }
}
