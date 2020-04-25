// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessors" />.
    /// </summary>
    [Singleton]
    public class EventProcessors : IEventProcessors
    {
        readonly ITenants _tenants;
        readonly FactoryFor<IStreamProcessors> _getStreamProcessors;
        readonly FactoryFor<IFetchEventsFromStreams> _getEventsFromStreamsFetcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessors"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        /// <param name="getStreamProcessors"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="getEventsFromStreamsFetcher"><see cref="FactoryFor{T}"/> the  <see cref="IFetchEventsFromStreams">fetcher</see> for writing events.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventProcessors(
            ITenants tenants,
            FactoryFor<IStreamProcessors> getStreamProcessors,
            FactoryFor<IFetchEventsFromStreams> getEventsFromStreamsFetcher,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _tenants = tenants;
            _getStreamProcessors = getStreamProcessors;
            _getEventsFromStreamsFetcher = getEventsFromStreamsFetcher;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<EventProcessorRegistrationResult> Register(StreamId sourceStreamId, IEventProcessor eventProcessor, StreamProcessorRegistrations streamProcessorRegistrations, CancellationToken cancellationToken)
        {
            try
            {
                StreamId targetStream = eventProcessor.Identifier.Value;

                var registrationResults = await RegisterStreamProcessorsForAllTenants(sourceStreamId, eventProcessor, cancellationToken).ConfigureAwait(false);
                registrationResults.ForEach(_ => streamProcessorRegistrations.Add(_.Item1, _.Item2));
                return new EventProcessorRegistrationResult();
            }
            catch (Exception ex)
            {
                return new EventProcessorRegistrationResult($"Failed registering Event Processor: '{eventProcessor.Identifier}' on Stream: '{sourceStreamId}. {ex.Message}'");
            }
        }

        async Task<IEnumerable<(TenantId, StreamProcessorRegistrationResult)>> RegisterStreamProcessorsForAllTenants(
            StreamId sourceStreamId,
            IEventProcessor eventProcessor,
            CancellationToken cancellationToken)
        {
            var registrationResults = new List<(TenantId, StreamProcessorRegistrationResult)>();
            foreach (var tenant in _tenants.All.ToArray())
            {
                _executionContextManager.CurrentFor(tenant);
                var registrationResult = await _getStreamProcessors().Register(eventProcessor, _getEventsFromStreamsFetcher(), sourceStreamId, cancellationToken).ConfigureAwait(false);
                registrationResults.Add((tenant, registrationResult));
            }

            return registrationResults.AsEnumerable();
        }
    }
}