// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessorsRegistration" /> that manages the registration of an Event Processor.
    /// </summary>
    public class EventProcessorRegistration : AbstractEventProcessorRegistration
    {
        readonly StreamId _sourceStreamId;
        readonly IEventProcessor _eventProcessor;
        readonly FactoryFor<IStreamDefinitions> _getStreamDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorRegistration"/> class.
        /// </summary>
        /// <param name="sourceStreamId">The source <see cref="StreamId" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="getStreamDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitions" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public EventProcessorRegistration(
            StreamId sourceStreamId,
            IEventProcessor eventProcessor,
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            FactoryFor<IStreamDefinitions> getStreamDefinitions,
            CancellationToken cancellationToken)
                : base(streamProcessorForAllTenants, cancellationToken)
        {
            _sourceStreamId = sourceStreamId;
            _eventProcessor = eventProcessor;
            _getStreamDefinitions = getStreamDefinitions;
        }

        /// <inheritdoc/>
        protected override async Task<EventProcessorsRegistrationResult> PerformRegistration()
        {
            try
            {
                StreamId targetStream = _eventProcessor.Identifier.Value;

                var failed = await RegisterStreamProcessor(_eventProcessor, () => _getStreamDefinitions().GetFor(_eventProcessor.Scope, _sourceStreamId, CancellationToken)).ConfigureAwait(false);
                if (failed)
                {
                    Succeeded = false;
                    return new EventProcessorsRegistrationResult($"Failed registering Event Processor: '{_eventProcessor.Identifier}' on Stream: '{_sourceStreamId}");
                }

                return new EventProcessorsRegistrationResult();
            }
            catch (Exception ex)
            {
                Succeeded = false;
                return new EventProcessorsRegistrationResult($"Failed registering Event Processor: '{_eventProcessor.Identifier}' on Stream: '{_sourceStreamId}. {ex.Message}'");
            }
        }

        /// <inheritdoc/>
        protected override Task OnCompleted() => Task.CompletedTask;
    }
}