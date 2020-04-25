// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    [SingletonPerTenant]
    public class Filters : IFilters
    {
        readonly IStreamProcessors _streamProcessors;
        readonly IFilterValidators _filterValidators;
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filters"/> class.
        /// </summary>
        /// <param name="streamProcessorsFactory"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="eventsFromStreamsFetcher"><see cref="FactoryFor{T}"/> the <see cref="IFetchEventsFromStreams">fetcher</see> for fetching events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public Filters(
            IStreamProcessors streamProcessorsFactory,
            IFilterValidators filterValidators,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            ILogger logger)
        {
            _streamProcessors = streamProcessorsFactory;
            _filterValidators = filterValidators;
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FilterRegistrationResult<TFilterDefinition>> Register<TFilterDefinition>(
            IFilterProcessor<TFilterDefinition> filterProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            var filterRegistrationResult = _streamProcessors.Register(filterProcessor, _eventsFromStreamsFetcher, filterProcessor.Definition.SourceStream, cancellationToken);
            var filterValidationResult = await _filterValidators.Validate(filterProcessor, cancellationToken).ConfigureAwait(false);
            var failureReason = FailedFilterRegistrationReason.FromRegistrationResults(filterRegistrationResult, filterValidationResult);

            return failureReason.IsSet switch
                {
                    true => new FilterRegistrationResult<TFilterDefinition>(failureReason),
                    _ => new FilterRegistrationResult<TFilterDefinition>(filterRegistrationResult.StreamProcessor, filterProcessor)
                };
        }
    }
}