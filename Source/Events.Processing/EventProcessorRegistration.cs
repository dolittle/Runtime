// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessorsRegistration" /> that manages the registration of an Event Processor.
    /// </summary>
    public class EventProcessorRegistration : AbstractEventProcessorsRegistration
    {
        readonly ScopeId _scopeId;
        readonly EventProcessorId _eventProcessorId;
        readonly StreamId _sourceStreamId;
        readonly Func<Task<IEventProcessor>> _createEventProcessor;
        readonly Func<ScopeId, StreamId, Task<StreamDefinition>> _getStreamDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorRegistration"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The source <see cref="StreamId" />.</param>
        /// <param name="createEventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="getStreamDefinitions">A <see cref="Func{TResult}" /> that returns a <see cref="Task" /> that, when resolved, returns the <see cref="StreamDefinition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public EventProcessorRegistration(
            ScopeId scopeId,
            EventProcessorId eventProcessorId,
            StreamId sourceStreamId,
            Func<Task<IEventProcessor>> createEventProcessor,
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            Func<ScopeId, StreamId, Task<StreamDefinition>> getStreamDefinitions,
            CancellationToken cancellationToken)
                : base(streamProcessorForAllTenants, cancellationToken)
        {
            _scopeId = scopeId;
            _eventProcessorId = eventProcessorId;
            _sourceStreamId = sourceStreamId;
            _createEventProcessor = createEventProcessor;
            _getStreamDefinitions = getStreamDefinitions;
        }

        /// <inheritdoc/>
        protected override async Task<EventProcessorsRegistrationResult> PerformRegistration()
        {
            try
            {
                var failed = await RegisterStreamProcessor(_createEventProcessor, () => _getStreamDefinitions(_scopeId, _sourceStreamId)).ConfigureAwait(false);
                if (failed)
                {
                    Succeeded = false;
                    return new EventProcessorsRegistrationResult($"Failed registering Event Processor: '{_eventProcessorId}''");
                }

                Succeeded = true;
                return new EventProcessorsRegistrationResult();
            }
            catch (Exception ex)
            {
                Succeeded = false;
                return new EventProcessorsRegistrationResult($"Failed registering Event Processor: '{_eventProcessorId}'. {ex.Message}'");
            }
        }
    }
}