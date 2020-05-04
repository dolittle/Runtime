// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    [Singleton]
    public class Filters : IFilters
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly IStreamProcessors _streamProcessors;
        readonly IFilterValidators _filterValidators;
        readonly FactoryFor<IFilterDefinitions> _getFilterDefinitions;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitions;
        readonly ILogger<Filters> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filters"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="getFilterDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IFilterDefinitions" />. </param>
        /// <param name="getStreamDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitionRepository" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public Filters(
            IPerformActionOnAllTenants onAllTenants,
            IStreamProcessors streamProcessors,
            IFilterValidators filterValidators,
            FactoryFor<IFilterDefinitions> getFilterDefinitions,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitions,
            ILogger<Filters> logger)
        {
            _onAllTenants = onAllTenants;
            _streamProcessors = streamProcessors;
            _filterValidators = filterValidators;
            _getFilterDefinitions = getFilterDefinitions;
            _getStreamDefinitions = getStreamDefinitions;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FilterRegistrationResult> TryRegister<TDefinition>(ScopeId scopeId, EventProcessorId eventProcessorId,  TDefinition filterDefinition, Func<IFilterProcessor<TDefinition>> getFilterProcessor, CancellationToken cancellationToken)
            where TDefinition : IFilterDefinition
        {
            var validationResult = await ValidateFilter(getFilterProcessor, cancellationToken).ConfigureAwait(false);
            if (!validationResult.Succeeded)
            {
                return new FilterRegistrationResult(validationResult);
            }

            var streamDefinition = new StreamDefinition(filterDefinition);

            var successfullyRegisteredStreamProcessor = _streamProcessors.TryRegister(
                scopeId,
                eventProcessorId,
                streamDefinition,
                getFilterProcessor,
                cancellationToken,
                out var streamProcessor);

            try
            {
                await _onAllTenants.PerformAsync(_ => _getStreamDefinitions().Persist(scopeId, streamDefinition, cancellationToken)).ConfigureAwait(false);
                return new FilterRegistrationResult(streamProcessor);
            }
            catch (Exception)
            {
                streamProcessor?.Unregister();
                throw;
            }
        }

        async Task<FilterValidationResult> ValidateFilter<TDefinition>(Func<IFilterProcessor<TDefinition>> getFilterProcessor, CancellationToken cancellationToken)
            where TDefinition : IFilterDefinition
        {
            FilterValidationResult result = default;
            await _onAllTenants.PerformAsync(async _ =>
                {
                    if (result != default) return;
                    var filterProcessor = getFilterProcessor();
                    var tryGetFilterDefinition = await _getFilterDefinitions().TryGetFromStream(filterProcessor.Scope, filterProcessor.Definition.TargetStream, cancellationToken).ConfigureAwait(false);
                    if (tryGetFilterDefinition.Success)
                    {
                        var validationResult = await _filterValidators.Validate(tryGetFilterDefinition.Result, filterProcessor, cancellationToken).ConfigureAwait(false);
                        if (!validationResult.Succeeded) result = validationResult;
                    }
                }).ConfigureAwait(false);

            return result == default ? new FilterValidationResult() : result;
        }
    }
}