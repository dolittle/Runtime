// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessorsRegistration" /> for Event Handler.
    /// </summary>
    public class EventHandlerRegistration : AbstractEventProcessorsRegistration
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly EventProcessorId _eventProcessorId;
        readonly ScopeId _scopeId;
        readonly TypeFilterWithEventSourcePartitionDefinition _filterDefinition;
        readonly Func<Task<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>>> _createFilterProcessor;
        readonly Func<Task<IEventProcessor>> _createEventProcessor;
        readonly IFilterValidators _filterValidators;
        readonly IRegisterStreamProcessorForAllTenants _streamProcessorForAllTenants;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitionRepository;

        EventProcessorRegistration _eventProcessorRegistration;
        FilterRegistration<TypeFilterWithEventSourcePartitionDefinition> _filterRegistration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerRegistration"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="filterDefinition">The <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <param name="createFilterProcessor">A <see cref="Func{T, TResult}" /> that returns a <see cref="Task" /> that, when resolved, returns the <see cref="TypeFilterWithEventSourcePartition" />.</param>
        /// <param name="createEventProcessor">A <see cref="Func{T, TResult}" /> that returns a <see cref="Task" /> that, when resolved, returns the <see cref="IEventProcessor" />.</param>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="getStreamDefinitionRepository">The <see cref="FactoryFor{T}" /> <see cref=" IStreamDefinitionRepository" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public EventHandlerRegistration(
            ScopeId scopeId,
            EventProcessorId eventProcessorId,
            TypeFilterWithEventSourcePartitionDefinition filterDefinition,
            Func<Task<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>>> createFilterProcessor,
            Func<Task<IEventProcessor>> createEventProcessor,
            IPerformActionOnAllTenants onAllTenants,
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitionRepository,
            IFilterValidators filterValidators,
            CancellationToken cancellationToken)
            : base(streamProcessorForAllTenants, cancellationToken)
        {
            _eventProcessorId = eventProcessorId;
            _scopeId = scopeId;
            _filterDefinition = filterDefinition;
            _createFilterProcessor = createFilterProcessor;
            _streamProcessorForAllTenants = streamProcessorForAllTenants;
            _getStreamDefinitionRepository = getStreamDefinitionRepository;
            _createEventProcessor = createEventProcessor;
            _filterValidators = filterValidators;
            _onAllTenants = onAllTenants;
        }

        /// <inheritdoc/>
        protected override async Task<EventProcessorsRegistrationResult> PerformRegistration()
        {
            try
            {
                _filterRegistration = new FilterRegistration<TypeFilterWithEventSourcePartitionDefinition>(
                    _scopeId,
                    _filterDefinition,
                    _createFilterProcessor,
                    _onAllTenants,
                    _streamProcessorForAllTenants,
                    _filterValidators,
                    _getStreamDefinitionRepository,
                    CancellationToken);
                _eventProcessorRegistration = new EventProcessorRegistration(
                    _scopeId,
                    _eventProcessorId,
                    _filterDefinition.TargetStream,
                    _createEventProcessor,
                    _streamProcessorForAllTenants,
                    (_, __) => Task.FromResult(new StreamDefinition(_filterDefinition)),
                    CancellationToken);

                var filterRegistrationResult = await _filterRegistration.Register().ConfigureAwait(false);
                if (!filterRegistrationResult.Succeeded)
                {
                    Succeeded = false;
                    return new EventProcessorsRegistrationResult($"Failed to register Event Handler: '{_eventProcessorId}. {filterRegistrationResult.FailureReason}");
                }

                var eventProcessorRegistrationResult = await _eventProcessorRegistration.Register().ConfigureAwait(false);
                if (!eventProcessorRegistrationResult.Succeeded)
                {
                    Succeeded = false;
                    return new EventProcessorsRegistrationResult($"Failed to register Event Handler: '{_eventProcessorId}. {eventProcessorRegistrationResult.FailureReason}");
                }

                Succeeded = true;
                return new EventProcessorsRegistrationResult();
            }
            catch (Exception ex)
            {
                Succeeded = false;
                return new EventProcessorsRegistrationResult($"Failed to register Event Handler: '{_eventProcessorId}. {ex.Message}'");
            }
        }

        /// <inheritdoc/>
        protected override async Task OnCompleted()
        {
            if (Succeeded)
            {
                await _filterRegistration.Complete().ConfigureAwait(false);
                await _eventProcessorRegistration.Complete().ConfigureAwait(false);
            }
            else
            {
                if (_filterRegistration != null) await _filterRegistration.Fail().ConfigureAwait(false);
                if (_eventProcessorRegistration != null) await _eventProcessorRegistration.Fail().ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (Disposed) return;
            base.Dispose(disposing);
            if (disposing)
            {
                _filterRegistration?.Dispose();
                _eventProcessorRegistration?.Dispose();
            }
        }
    }
}