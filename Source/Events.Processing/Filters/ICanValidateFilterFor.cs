// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Defines a system that can validate a <see cref="IFilterProcessor{TDefinition}" /> for a specific processor.
/// </summary>
/// <typeparam name="TDefinition">The subtype of <see cref="IFilterDefinition" />.</typeparam>
public interface ICanValidateFilterFor<TDefinition>
    where TDefinition : IFilterDefinition
{
    /// <summary>
    /// Validates a filter of with definition <see typeparam="TDefinition" />.
    /// </summary>
    /// <param name="persistedDefinition">The persisted <typeparamref name="TDefinition"/>.</param>
    /// <param name="filter">The <see cref="IFilterProcessor{TDefinition}" />.</param>
    /// <param name="lastUnprocessedEvent">The <see cref="StreamPosition" /> of the last unprocessed event.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns><see cref="FilterValidationResult" />.</returns>
    Task<FilterValidationResult> Validate(TDefinition persistedDefinition, IFilterProcessor<TDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken);
}