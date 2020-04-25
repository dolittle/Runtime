// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessorsRegistration" /> that manages the registration of a Filter.
    /// </summary>
    /// <typeparam name="TFilterDefinition">The <see cref="IFilterDefinition" /> type.</typeparam>
    public class FilterRegistration<TFilterDefinition> : AbstractEventProcessorsRegistration
        where TFilterDefinition : IFilterDefinition
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly IFilterProcessor<TFilterDefinition> _filterProcessor;
        readonly IFilterValidators _filterValidators;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegistration{T}"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="filterProcessor">The <see cref="IFilterProcessor{TDefinition}" />.</param>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="getStreamDefinitionRepository">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitionRepository" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public FilterRegistration(
            IPerformActionOnAllTenants onAllTenants,
            IFilterProcessor<TFilterDefinition> filterProcessor,
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            IFilterValidators filterValidators,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitionRepository,
            CancellationToken cancellationToken)
            : base(streamProcessorForAllTenants, cancellationToken)
        {
            _onAllTenants = onAllTenants;
            _filterProcessor = filterProcessor;
            _filterValidators = filterValidators;
            _getStreamDefinitionRepository = getStreamDefinitionRepository;
        }

        /// <inheritdoc/>
        protected override Task OnCompleted() =>
            _onAllTenants.PerformAsync(_ => _getStreamDefinitionRepository().Persist(_filterProcessor.Scope, new StreamDefinition(_filterProcessor.Definition), CancellationToken.None));

        /// <inheritdoc/>
        protected override async Task<EventProcessorsRegistrationResult> PerformRegistration()
        {
            try
            {
                if (_filterProcessor.Scope == ScopeId.Default && _filterProcessor.Definition.TargetStream == StreamId.AllStreamId) throw new FilterCannotWriteToEventLog();
                var validationResults = await ValidateFilter().ConfigureAwait(false);
                var filterValidationResult = await _filterValidators.Validate(_filterProcessor, CancellationToken).ConfigureAwait(false);
                if (!filterValidationResult.Succeeded)
                {
                    Succeeded = false;
                    return new EventProcessorsRegistrationResult($"Failed to register Filter: '{_filterProcessor.Identifier}' on Stream: {_filterProcessor.Definition.SourceStream}. {filterValidationResult.FailureReason}");
                }

                var failed = await RegisterStreamProcessor(
                    _filterProcessor,
                    () => _getStreamDefinitionRepository().GetFor(_filterProcessor.Scope, _filterProcessor.Definition.SourceStream, CancellationToken)).ConfigureAwait(false);

                if (failed)
                {
                    Succeeded = false;
                    return new EventProcessorsRegistrationResult($"Failed registering Filter: '{_filterProcessor.Identifier}' on Stream: '{_filterProcessor.Definition.SourceStream}");
                }

                Succeeded = true;
                return new EventProcessorsRegistrationResult();
            }
            catch (Exception ex)
            {
                Succeeded = false;
                return new EventProcessorsRegistrationResult($"Failed to register Filter: '{_filterProcessor.Identifier}' on Stream: {_filterProcessor.Definition.SourceStream}. {ex.Message}");
            }
        }

        async Task<IEnumerable<FilterValidationResult>> ValidateFilter()
        {
            var validationResults = new List<FilterValidationResult>();
            await _onAllTenants.PerformAsync(
                async (_) =>
                {
                    var result = await _filterValidators.Validate(_filterProcessor, CancellationToken.None).ConfigureAwait(false);
                    validationResults.Add(result);
                }).ConfigureAwait(false);
            return validationResults;
        }
    }
}