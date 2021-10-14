// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using ReverseCallDispatcherType = Dolittle.Runtime.Services.IReverseCallDispatcher<
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
                                    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;
namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHandlers"/>.
    /// </summary>
    [Singleton]
    public class EventHandlers : IEventHandlers
    {
        readonly ConcurrentDictionary<EventHandlerId, EventHandler> _eventHandlers = new();
        
        readonly IStreamProcessors _streamProcessors;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly IStreamDefinitions _streamDefinitions;
        readonly ILoggerFactory _loggerFactory;
        readonly IValidateFilterForAllTenants _filterValidator;
        readonly ILogger<EventHandlers> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="streamDefinitions">The<see cref="IStreamDefinitions" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public EventHandlers(
            IValidateFilterForAllTenants filterForAllTenants,
            IStreamProcessors streamProcessors,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            IStreamDefinitions streamDefinitions,
            ILoggerFactory loggerFactory)
        {
            _filterValidator = filterForAllTenants;
            _streamProcessors = streamProcessors;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _streamDefinitions = streamDefinitions;
            _loggerFactory = loggerFactory;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _streamDefinitions = streamDefinitions;
            _streamProcessors = streamProcessors;
            _logger = loggerFactory.CreateLogger<EventHandlers>();
        }
        
        /// <inheritdoc />
        public async Task RegisterAndStart(ReverseCallDispatcherType dispatcher, EventHandlerRegistrationArguments arguments, CancellationToken cancellationToken)
        {
            using var eventHandler = new EventHandler(
                _streamProcessors,
                _filterValidator,
                _streamDefinitions,
                dispatcher,
                arguments,
                _getEventsToStreamsWriter,
                _loggerFactory,
                cancellationToken);
            var eventHandlerId = new EventHandlerId(eventHandler.Scope, eventHandler.EventProcessor.Value);
            if (!_eventHandlers.TryAdd(eventHandlerId, eventHandler))
            {
                throw new EventHandlerAlreadyRegistered(eventHandlerId);
            }
            await eventHandler.RegisterAndStart().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<Try<StreamPosition>> SetToPosition(EventHandlerId eventHandlerId, TenantId tenant, StreamPosition position)
            => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
                ? eventHandler.SetToPosition(tenant, position)
                : Task.FromResult<Try<StreamPosition>>(new EventHandlerNotRegistered(eventHandlerId));

        /// <inheritdoc />
        public Task<Try<IDictionary<TenantId, Try<StreamPosition>>>> SetToInitialForAllTenants(EventHandlerId eventHandlerId)
            => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
                ? eventHandler.SetToInitialForAllTenants()
                : Task.FromResult<Try<IDictionary<TenantId, Try<StreamPosition>>>>(new EventHandlerNotRegistered(eventHandlerId));
    }
}