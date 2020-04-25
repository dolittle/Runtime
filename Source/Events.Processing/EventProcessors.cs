// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessors" />.
    /// </summary>
    public class EventProcessors : IEventProcessors
    {
        readonly IRegisterStreamProcessorForAllTenants _streamProcessorForAllTenants;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessors"/> class.
        /// </summary>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventProcessors(
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            ILogger<EventProcessors> logger)
        {
            _streamProcessorForAllTenants = streamProcessorForAllTenants;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<EventProcessorRegistrationResult> Register(StreamId sourceStreamId, IEventProcessor eventProcessor, StreamProcessorRegistrations streamProcessorRegistrations, CancellationToken cancellationToken)
        {
            try
            {
                StreamId targetStream = eventProcessor.Identifier.Value;

                var registrationResults = await _streamProcessorForAllTenants.Register(eventProcessor, sourceStreamId, cancellationToken).ConfigureAwait(false);
                registrationResults.ForEach(streamProcessorRegistrations.Add);
                return new EventProcessorRegistrationResult();
            }
            catch (Exception ex)
            {
                return new EventProcessorRegistrationResult($"Failed registering Event Processor: '{eventProcessor.Identifier}' on Stream: '{sourceStreamId}. {ex.Message}'");
            }
        }
    }
}