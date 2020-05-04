// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Defines a system that knows about Event Handlers.
    /// </summary>
    public interface IEventHandlers
    {
        /// <summary>
        /// Try to register Event Handler.
        /// </summary>
        /// <typeparam name="TFilterDefinition">The <see cref="IFilterDefinition" /> type.</typeparam>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="filterDefinition">The <typeparamref name="TFilterDefinition" />.</param>
        /// <param name="getFilterProcessor">The <see cref="Func{TResult}" /> that returns the <see cref="IFilterProcessor{TDefinition}" />.</param>
        /// <param name="getEventProcessor">The <see cref="Func{TResult}" /> that returns the <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="EventHandlersRegistrationResult" />.</returns>
        Task<EventHandlersRegistrationResult> TryRegister<TFilterDefinition>(
            ScopeId scopeId,
            EventProcessorId eventProcessorId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            Func<IEventProcessor> getEventProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition;
    }
}