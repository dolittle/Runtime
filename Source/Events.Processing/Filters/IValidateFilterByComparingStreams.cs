// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that validates a filter definition by comparing the result of running the filter again
    /// and checking that the resulting stream is equal to the previous one.
    /// </summary>
    public interface IValidateFilterByComparingStreams
    {
        /// <summary>
        /// Validate a Filter by comparing the generated Streams.
        /// </summary>
        /// <typeparam name="TFilterDefinition">The <see cref="IFilterDefinition" /> type.</typeparam>
        /// <param name="persistedDefinition">The persisted <see cref="IFilterDefinition" />.</param>
        /// <param name="filter">The <see cref="IFilterProcessor{TDefinition}" />.</param>
        /// <param name="lastUnprocessedEvent">The <see cref="StreamPosition" /> of the last unprocessed event.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="FilterValidationResult" />. </returns>
        Task<FilterValidationResult> Validate<TFilterDefinition>(IFilterDefinition persistedDefinition, IFilterProcessor<TFilterDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition;
    }
}
