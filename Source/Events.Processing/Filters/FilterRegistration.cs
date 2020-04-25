// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
        readonly ScopeId _scopeId;
        readonly TFilterDefinition _filterDefinition;
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly Func<Task<IFilterProcessor<TFilterDefinition>>> _createFilterProcessor;
        readonly IFilterValidators _filterValidators;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegistration{T}"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <param name="createFilterProcessor">A <see cref="Func{TResult}" /> that returns a <see cref="Task" /> that, when resolved, returns the <see cref="IFilterProcessor{TDefinition}" />.</param>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="getStreamDefinitionRepository">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitionRepository" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public FilterRegistration(
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<Task<IFilterProcessor<TFilterDefinition>>> createFilterProcessor,
            IPerformActionOnAllTenants onAllTenants,
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            IFilterValidators filterValidators,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitionRepository,
            CancellationToken cancellationToken)
            : base(streamProcessorForAllTenants, cancellationToken)
        {
            _scopeId = scopeId;
            _filterDefinition = filterDefinition;
            _onAllTenants = onAllTenants;
            _createFilterProcessor = createFilterProcessor;
            _filterValidators = filterValidators;
            _getStreamDefinitionRepository = getStreamDefinitionRepository;
        }

        /// <inheritdoc/>
        protected override Task OnCompleted()
        {
            if (Succeeded)
            {
                return _onAllTenants.PerformAsync(_ => _getStreamDefinitionRepository().Persist(_scopeId, new StreamDefinition(_filterDefinition), CancellationToken.None));
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override async Task<EventProcessorsRegistrationResult> PerformRegistration()
        {
            try
            {
                if (_scopeId == ScopeId.Default && _filterDefinition.TargetStream == StreamId.AllStreamId) throw new FilterCannotWriteToEventLog();
                var validationResults = await ValidateFilter().ConfigureAwait(false);
                if (validationResults.Any(_ => !_.Succeeded))
                {
                    Succeeded = false;
                    var failedValidation = validationResults.First(_ => !_.Succeeded);
                    return new EventProcessorsRegistrationResult($"Failed to register Filter: '{_filterDefinition.TargetStream}'. {failedValidation.FailureReason}");
                }

                var failed = await RegisterStreamProcessor(
                    _createFilterProcessor,
                    () => _getStreamDefinitionRepository().GetFor(_scopeId, _filterDefinition.SourceStream, CancellationToken.None)).ConfigureAwait(false);

                if (failed)
                {
                    Succeeded = false;
                    var failedRegistration = StreamProcessorRegistrations.First(_ => !_.Succeeded);
                    return new EventProcessorsRegistrationResult($"Failed registering Filter: '{_filterDefinition.TargetStream}'. {failedRegistration.FailureReason}");
                }

                Succeeded = true;
                return new EventProcessorsRegistrationResult();
            }
            catch (Exception ex)
            {
                Succeeded = false;
                return new EventProcessorsRegistrationResult($"Failed to register Filter: '{_filterDefinition.TargetStream}'. {ex.Message}");
            }
        }

        async Task<IEnumerable<FilterValidationResult>> ValidateFilter()
        {
            var validationResults = new List<FilterValidationResult>();
            await _onAllTenants.PerformAsync(
                async (_) =>
                {
                    var eventProcessor = await _createFilterProcessor().ConfigureAwait(false);
                    var result = await _filterValidators.Validate(eventProcessor, CancellationToken.None).ConfigureAwait(false);
                    validationResults.Add(result);
                }).ConfigureAwait(false);
            return validationResults;
        }
    }
}