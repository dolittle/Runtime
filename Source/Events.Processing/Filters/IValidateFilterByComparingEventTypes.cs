// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that validates a filter by checking that the added or removed event types would be present
    /// in a stream created with the new filter definition.
    /// </summary>
    public interface IValidateFilterByComparingEventTypes
    {
        /// <summary>
        /// Validate a Filter by comparing the generated Streams.
        /// </summary>
        /// <param name="persistedDefinition">The persisted <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <param name="filter">The <see cref="IFilterProcessor{TDefinition}" /> of <see cref="TypeFilterWithEventSourcePartitionDefinition"/>.</param>
        /// <param name="lastUnprocessedEvent">The <see cref="StreamPosition" /> of the last unprocessed event.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="FilterValidationResult" />. </returns>
        Task<FilterValidationResult> Validate(TypeFilterWithEventSourcePartitionDefinition persistedDefinition, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken);
    }
}
