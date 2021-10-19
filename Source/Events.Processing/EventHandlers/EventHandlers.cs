// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<EventHandlerInfo> All => _eventHandlers.Select(_ => _.Value.Info);

        /// <inheritdoc />
        public Try<IDictionary<TenantId, IStreamProcessorState>> CurrentStateFor(EventHandlerId eventHandlerId)
            => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
                ? eventHandler.GetEventHandlerCurrentState()
                : new EventHandlerNotRegistered(eventHandlerId);
        
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
            try
            {
                await eventHandler.RegisterAndStart().ConfigureAwait(false);
            }
            finally
            {
                _eventHandlers.Remove(eventHandlerId, out _);
            }
        }

        /// <inheritdoc />
        public Task<Try<StreamPosition>> ReprocessEventsFrom(EventHandlerId eventHandlerId, TenantId tenant, StreamPosition position)
            => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
                ? eventHandler.ReprocessEventsFrom(tenant, position)
                : Task.FromResult<Try<StreamPosition>>(new EventHandlerNotRegistered(eventHandlerId));

        /// <inheritdoc />
        public Task<Try<IDictionary<TenantId, Try<StreamPosition>>>> ReprocessAllEvents(EventHandlerId eventHandlerId)
            => _eventHandlers.TryGetValue(eventHandlerId, out var eventHandler)
                ? eventHandler.ReprocessAllEvents()
                : Task.FromResult<Try<IDictionary<TenantId, Try<StreamPosition>>>>(new EventHandlerNotRegistered(eventHandlerId));
    }
}