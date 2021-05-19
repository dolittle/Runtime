// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters.EventHorizon;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;
using Dolittle.Runtime.Events.Processing.Filters.Unpartitioned;
using Dolittle.Runtime.Events.Processing.Filters.Partitioned;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    public class FiltersService : FiltersBase
    {
        readonly IHostApplicationLifetime _hostApplicationLifetime;
        readonly IStreamProcessors _streamProcessors;
        readonly IValidateFilterForAllTenants _filterForAllTenants;
        readonly IExecutionContextManager _executionContextManager;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly FactoryFor<IWriteEventsToPublicStreams> _getEventsToPublicStreamsWriter;
        readonly IInitiateReverseCallServices _reverseCallServices;
        readonly IUnpartitionedFiltersProtocol _unpartitionedFiltersProtocol;
        readonly IPartitionedFiltersProtocol _partitionedFiltersProtocol;
        readonly IPublicFiltersProtocol _publicFiltersProtocol;
        readonly IStreamDefinitions _streamDefinitions;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="streamDefinitions">The <see cref="IFilterDefinitions" />.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="getEventsToPublicStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToPublicStreams" />.</param>
        /// <param name="reverseCallServices">The <see cref="IInitiateReverseCallServices" />.</param>
        /// <param name="unpartitionedFiltersProtocol">The <see cref="IUnpartitionedFiltersProtocol" />.</param>
        /// <param name="partitionedFiltersProtocol">The <see cref="IPartitionedFiltersProtocol" />.</param>
        /// <param name="publicFiltersProtocol">The <see cref="IPublicFiltersProtocol" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public FiltersService(
            IHostApplicationLifetime hostApplicationLifetime,
            IStreamProcessors streamProcessors,
            IValidateFilterForAllTenants filterForAllTenants,
            IExecutionContextManager executionContextManager,
            IStreamDefinitions streamDefinitions,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            FactoryFor<IWriteEventsToPublicStreams> getEventsToPublicStreamsWriter,
            IInitiateReverseCallServices reverseCallServices,
            IUnpartitionedFiltersProtocol unpartitionedFiltersProtocol,
            IPartitionedFiltersProtocol partitionedFiltersProtocol,
            IPublicFiltersProtocol publicFiltersProtocol,
            ILoggerFactory loggerFactory)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _streamProcessors = streamProcessors;
            _filterForAllTenants = filterForAllTenants;
            _executionContextManager = executionContextManager;
            _streamDefinitions = streamDefinitions;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _getEventsToPublicStreamsWriter = getEventsToPublicStreamsWriter;
            _reverseCallServices = reverseCallServices;
            _unpartitionedFiltersProtocol = unpartitionedFiltersProtocol;
            _partitionedFiltersProtocol = partitionedFiltersProtocol;
            _publicFiltersProtocol = publicFiltersProtocol;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<FiltersService>();
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<FilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.FilterConnectionRequestedFor("Unpartitioned");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
            var tryConnect = await _reverseCallServices.Connect(
                runtimeStream,
                clientStream,
                context,
                _unpartitionedFiltersProtocol,
                cts.Token).ConfigureAwait(false);

            if (!tryConnect.Success) return;
            var (dispatcher, arguments) = tryConnect.Result;

            _logger.SettingExecutionContext(arguments.ExecutionContext);
            _executionContextManager.CurrentFor(arguments.ExecutionContext);

            if (await RejectIfInvalidFilterId(dispatcher, arguments.Filter, cts.Token).ConfigureAwait(false)) return;

            var filterDefinition = new FilterDefinition(StreamId.EventLog, arguments.Filter.Value, false);
            await RegisterFilter(
                dispatcher,
                arguments.Scope,
                filterDefinition,
                () => new Unpartitioned.FilterProcessor(
                    arguments.Scope,
                    filterDefinition,
                    dispatcher,
                    _getEventsToStreamsWriter(),
                    _loggerFactory.CreateLogger<Unpartitioned.FilterProcessor>()),
                cts.Token).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task ConnectPartitioned(
            IAsyncStreamReader<PartitionedFilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.FilterConnectionRequestedFor("Partitioned");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
            var tryConnect = await _reverseCallServices.Connect(
                runtimeStream,
                clientStream,
                context,
                _partitionedFiltersProtocol,
                cts.Token).ConfigureAwait(false);

            if (!tryConnect.Success) return;
            var (dispatcher, arguments) = tryConnect.Result;

            _logger.SettingExecutionContext(arguments.ExecutionContext);
            _executionContextManager.CurrentFor(arguments.ExecutionContext);
            if (await RejectIfInvalidFilterId(dispatcher, arguments.Filter, cts.Token).ConfigureAwait(false)) return;

            var filterDefinition = new FilterDefinition(StreamId.EventLog, arguments.Filter.Value, true);

            await RegisterFilter(
                dispatcher,
                arguments.Scope,
                filterDefinition,
                () => new Partitioned.FilterProcessor(
                    arguments.Scope,
                    filterDefinition,
                    dispatcher,
                    _getEventsToStreamsWriter(),
                    _loggerFactory.CreateLogger<Partitioned.FilterProcessor>()),
                cts.Token).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task ConnectPublic(
            IAsyncStreamReader<PublicFilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.FilterConnectionRequestedFor("Public");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
            var tryConnect = await _reverseCallServices.Connect(
                runtimeStream,
                clientStream,
                context,
                _publicFiltersProtocol,
                cts.Token).ConfigureAwait(false);

            if (!tryConnect.Success) return;
            var (dispatcher, arguments) = tryConnect.Result;

            _logger.SettingExecutionContext(arguments.ExecutionContext);
            _executionContextManager.CurrentFor(arguments.ExecutionContext);

            if (await RejectIfInvalidFilterId(dispatcher, arguments.Filter, cts.Token).ConfigureAwait(false)) return;

            var filterDefinition = new PublicFilterDefinition(StreamId.EventLog, arguments.Filter.Value);
            await RegisterFilter(
                dispatcher,
                ScopeId.Default,
                filterDefinition,
                () => new PublicFilterProcessor(
                    filterDefinition,
                    dispatcher,
                    _getEventsToPublicStreamsWriter(),
                    _loggerFactory.CreateLogger<PublicFilterProcessor>()),
                cts.Token).ConfigureAwait(false);
        }

        async Task<bool> RejectIfInvalidFilterId<TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            EventProcessorId filterId,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
        {
            StreamId filterStream = filterId.Value;
            if (filterStream.IsNonWriteable)
            {
                _logger.FilterIsInvalid(filterStream);
                var failure = new Failure(FiltersFailures.CannotRegisterFilterOnNonWriteableStream, $"Filter Id: '{filterId.Value}' is an invalid Stream Id");
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return true;
            }

            return false;
        }

        async Task RegisterFilter<TFilterDefinition, TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            CancellationToken externalCancellationToken)
            where TFilterDefinition : IFilterDefinition
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
        {
            _logger.ReceivedFilter(filterDefinition.SourceStream, filterDefinition.TargetStream, scopeId);
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken);
            var cancellationToken = linkedTokenSource.Token;

            _logger.ConnectingFilter(filterDefinition.TargetStream);

            var tryRegisterFilter = TryRegisterStreamProcessor(scopeId, filterDefinition, getFilterProcessor, cancellationToken);
            if (!tryRegisterFilter.Success)
            {
                linkedTokenSource.Cancel();

                if (tryRegisterFilter.Exception is StreamProcessorAlreadyRegistered)
                {
                    _logger.FilterAlreadyRegistered(filterDefinition.TargetStream);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Filter '{filterDefinition.TargetStream}'. Filter already registered.");
                    await WriteFailedRegistrationResponse(dispatcher, failure, externalCancellationToken).ConfigureAwait(false);
                    return;
                }
                else
                {
                    var exception = tryRegisterFilter.Exception;
                    _logger.ErrorWhileRegisteringFilter(exception, filterDefinition.TargetStream);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
            }

            using var filterStreamProcessor = tryRegisterFilter.Result;
            var tryStartFilter = await TryStartFilter(
                dispatcher,
                filterStreamProcessor,
                scopeId,
                filterDefinition,
                getFilterProcessor,
                cancellationToken).ConfigureAwait(false);
            if (!tryStartFilter.Success)
            {
                linkedTokenSource.Cancel();
                var exception = tryStartFilter.Exception;
                _logger.ErrorWhileStartingFilter(exception, filterDefinition.TargetStream, scopeId);
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            var tasks = tryStartFilter.Result;
            try
            {
                await Task.WhenAny(tasks).ConfigureAwait(false);
                if (TryGetException(tasks, out var ex))
                {
                    _logger.ErrorWhileRunningFilter(ex, filterDefinition.TargetStream, scopeId);
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
            finally
            {
                linkedTokenSource.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                _logger.FilterStopped(filterDefinition.TargetStream, scopeId);
            }
        }

        async Task<Try<IEnumerable<Task>>> TryStartFilter<TClientMessage, TConnectRequest, TResponse, TFilterDefinition>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            StreamProcessor streamProcessor,
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
            where TFilterDefinition : IFilterDefinition
        {
            _logger.StartingFilter(filterDefinition.TargetStream);
            try
            {
                var runningDispatcher = dispatcher.Accept(new FilterRegistrationResponse(), cancellationToken);
                await streamProcessor.Initialize().ConfigureAwait(false);
                await ValidateFilter(
                    scopeId,
                    filterDefinition,
                    getFilterProcessor,
                    cancellationToken).ConfigureAwait(false);
                return new[] { streamProcessor.Start(), runningDispatcher };
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.ErrorWhileStartingFilter(ex, filterDefinition.TargetStream, scopeId);
                }

                return ex;
            }
        }

        Try<StreamProcessor> TryRegisterStreamProcessor<TFilterDefinition>(
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            _logger.RegisteringStreamProcessorForFilter(filterDefinition.TargetStream, filterDefinition.SourceStream);
            try
            {
                return _streamProcessors.TryCreateAndRegister(
                    scopeId,
                    filterDefinition.TargetStream.Value,
                    new EventLogStreamDefinition(),
                    () => getFilterProcessor(),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.ErrorWhileRegisteringStreamProcessorForFilter(ex, filterDefinition.TargetStream, filterDefinition.SourceStream);
                }

                return ex;
            }
        }

        async Task ValidateFilter<TFilterDefinition>(
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            _logger.ValidatingFilter(filterDefinition.TargetStream);
            var filterValidationResults = await _filterForAllTenants.Validate(getFilterProcessor, cancellationToken).ConfigureAwait(false);

            if (filterValidationResults.Any(_ => !_.Value.Success))
            {
                var firstFailedValidation = filterValidationResults.Select(_ => _.Value).First(_ => !_.Success);
                _logger.FilterValidationFailed(filterDefinition.TargetStream, firstFailedValidation.FailureReason);
                throw new FilterValidationFailed(filterDefinition.TargetStream, firstFailedValidation.FailureReason);
            }

            var filteredStreamDefinition = new StreamDefinition(filterDefinition);
            _logger.PersistingStreamDefinition(filteredStreamDefinition.StreamId);
            await _streamDefinitions.Persist(scopeId, filteredStreamDefinition, cancellationToken).ConfigureAwait(false);
        }

        bool TryGetException(IEnumerable<Task> tasks, out Exception exception)
        {
            exception = tasks.FirstOrDefault(_ => _.Exception != default)?.Exception;
            if (exception != default)
            {
                while (exception.InnerException != null) exception = exception.InnerException;
            }

            return exception != default;
        }

        Task WriteFailedRegistrationResponse<TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class => dispatcher.Reject(new FilterRegistrationResponse { Failure = failure }, cancellationToken);
    }
}
