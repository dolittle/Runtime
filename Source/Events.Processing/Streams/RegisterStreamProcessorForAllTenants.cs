// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IRegisterStreamProcessorForAllTenants" />.
    /// </summary>
    [Singleton]
    public class RegisterStreamProcessorForAllTenants : IRegisterStreamProcessorForAllTenants
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly FactoryFor<IStreamProcessors> _getStreamProcessors;
        readonly FactoryFor<IFetchEventsFromStreams> _getEventsFromStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterStreamProcessorForAllTenants"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="getStreamProcessors">The <see cref="FactoryFor{T}" /> <see cref="IStreamProcessors" />.</param>
        /// <param name="getEventsFromStreamsFetcher">The <see cref="FactoryFor{T}" /> <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public RegisterStreamProcessorForAllTenants(
            IPerformActionOnAllTenants onAllTenants,
            FactoryFor<IStreamProcessors> getStreamProcessors,
            FactoryFor<IFetchEventsFromStreams> getEventsFromStreamsFetcher,
            ILogger<RegisterStreamProcessorForAllTenants> logger)
        {
            _onAllTenants = onAllTenants;
            _getStreamProcessors = getStreamProcessors;
            _getEventsFromStreamsFetcher = getEventsFromStreamsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<StreamProcessorRegistration>> Register(IEventProcessor eventProcessor, Task<StreamDefinition> streamDefinitionGetter, CancellationToken cancellationToken)
        {
            _logger.Trace("Registering Stream Processor for Event Processor: {eventProcessor} for all Tenants", eventProcessor.Identifier);
            var registrationResults = new List<StreamProcessorRegistration>();
            await _onAllTenants.PerformAsync(async _ =>
                {
                    var streamDefinition = await streamDefinitionGetter.ConfigureAwait(false);
                    var registrationResult = _getStreamProcessors().Register(streamDefinition, eventProcessor, _getEventsFromStreamsFetcher(), cancellationToken);
                    registrationResults.Add(registrationResult);
                }).ConfigureAwait(false);
            return registrationResults;
        }
    }
}