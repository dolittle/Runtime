// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHandlers" />.
    /// </summary>
    [Singleton]
    public class EventHandlers : IEventHandlers
    {
        readonly IFilters _filters;
        readonly IEventProcessors _eventProcessors;
        readonly ILogger<EventHandlers> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="filters">The <see cref="IFilters" />.</param>
        /// <param name="eventProcessors">The <see cref="IEventHandlers" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHandlers(IFilters filters, IEventProcessors eventProcessors, ILogger<EventHandlers> logger)
        {
            _filters = filters;
            _eventProcessors = eventProcessors;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<EventHandlersRegistrationResult> TryRegister<TFilterDefinition>(
            ScopeId scopeId,
            EventProcessorId eventProcessorId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            Func<IEventProcessor> getEventProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            var streamDefinition = new StreamDefinition(filterDefinition);
            var eventProcessorRegistrationResult = _eventProcessors.Register(scopeId, eventProcessorId, streamDefinition.StreamId, getEventProcessor, cancellationToken);
            if (!eventProcessorRegistrationResult.Success)
            {
                return new EventHandlersRegistrationResult(eventProcessorRegistrationResult);
            }

            try
            {
                var filterRegistrationResult = await _filters.TryRegister(scopeId, eventProcessorId,  filterDefinition, getFilterProcessor, cancellationToken).ConfigureAwait(false);
                return new EventHandlersRegistrationResult(eventProcessorRegistrationResult, filterRegistrationResult);
            }
            catch (Exception)
            {
                eventProcessorRegistrationResult.StreamProcessor?.Unregister();
                throw;
            }
        }
    }
}