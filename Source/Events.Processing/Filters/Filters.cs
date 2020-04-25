// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    [Singleton]
    public class Filters : IFilters
    {
        readonly ITenants _tenants;
        readonly IExecutionContextManager _executionContextManager;
        readonly IFilterValidators _filterValidators;
        readonly FactoryFor<IStreamProcessors> _getStreamProcessors;
        readonly FactoryFor<IFetchEventsFromStreams> _getEventsFromStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filters"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="getStreamProcessors"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="getEventsFromStreamsFetcher"><see cref="FactoryFor{T}"/> the <see cref="IFetchEventsFromStreams">fetcher</see> for fetching events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public Filters(
            ITenants tenants,
            IExecutionContextManager executionContextManager,
            IFilterValidators filterValidators,
            FactoryFor<IStreamProcessors> getStreamProcessors,
            FactoryFor<IFetchEventsFromStreams> getEventsFromStreamsFetcher,
            ILogger logger)
        {
            _tenants = tenants;
            _executionContextManager = executionContextManager;
            _getStreamProcessors = getStreamProcessors;
            _filterValidators = filterValidators;
            _getEventsFromStreamsFetcher = getEventsFromStreamsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FilterRegistrationResult<TFilterDefinition>> Register<TFilterDefinition>(
            IFilterProcessor<TFilterDefinition> filterProcessor,
            StreamProcessorRegistrations streamProcessorRegistrations,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            try
            {
                var filterRegistrationResult = RegisterStreamProcessorsForAllTenants()
                var filterValidationResult = await _filterValidators.Validate(filterProcessor, cancellationToken).ConfigureAwait(false);

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