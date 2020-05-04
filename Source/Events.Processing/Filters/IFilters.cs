// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that knows about Filters.
    /// </summary>
    public interface IFilters
    {
        /// <summary>
        /// Try to register a Filter.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="filterDefinition">The <typeparamref name="TDefinition" /> <see cref="IFilterDefinition" />.</param>
        /// <param name="getFilterProcessor">The <see cref="Func{TResult}" /> that returns the <see cref="IFilterProcessor{TDefinition}" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <typeparam name="TDefinition">The <see cref="IFilterDefinition" /> type.</typeparam>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="FilterRegistrationResult" />.</returns>
        Task<FilterRegistrationResult> TryRegister<TDefinition>(ScopeId scopeId, EventProcessorId eventProcessorId,  TDefinition filterDefinition, Func<IFilterProcessor<TDefinition>> getFilterProcessor, CancellationToken cancellationToken)
            where TDefinition : IFilterDefinition;
    }
}
