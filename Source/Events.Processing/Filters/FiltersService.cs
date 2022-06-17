// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Rudimentary;
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
using Dolittle.Runtime.Events.Processing.Filters.Unpartitioned;
using Dolittle.Runtime.Events.Processing.Filters.Partitioned;
using Dolittle.Runtime.Services.Hosting;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using UnpartitionedFilterDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.FilterClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.FilterRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.FilterRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.FilterRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.FilterEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.FilterResponse>;
using PartitionedFilterDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.PartitionedFilterClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.FilterRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.PartitionedFilterRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.FilterRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.FilterEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.PartitionedFilterResponse>;
using PublicFilterDispatcher = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.PublicFilterClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.FilterRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.PublicFilterRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.FilterRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.FilterEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.PartitionedFilterResponse>;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents the implementation of <see creF="FiltersBase"/>.
/// </summary>
[PrivateService]
public class FiltersService : FiltersBase
{
    readonly IHostApplicationLifetime _hostApplicationLifetime;
    readonly IStreamProcessors _streamProcessors;
    readonly IValidateFilterForAllTenants _filterForAllTenants;
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly IInitiateReverseCallServices _reverseCallServices;
    readonly IUnpartitionedFiltersProtocol _unpartitionedFiltersProtocol;
    readonly IPartitionedFiltersProtocol _partitionedFiltersProtocol;
    readonly IPublicFiltersProtocol _publicFiltersProtocol;
    readonly Func<TenantId, ScopeId, FilterDefinition, UnpartitionedFilterDispatcher, Unpartitioned.FilterProcessor> _createUnpartitionedFilterProcessorFor;
    readonly Func<TenantId, ScopeId, FilterDefinition, PartitionedFilterDispatcher, Partitioned.FilterProcessor> _createPartitionedFilterProcessorFor;
    readonly Func<TenantId, PublicFilterDefinition, PublicFilterDispatcher, PublicFilterProcessor> _createPublicFilterProcessorFor;
    readonly IStreamDefinitions _streamDefinitions;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FiltersService"/> class.
    /// </summary>
    /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
    /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
    /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
    /// <param name="executionContextCreator">The <see cref="ICreateExecutionContexts" />.</param>
    /// <param name="streamDefinitions">The <see cref="IFilterDefinitions" />.</param>
    /// <param name="reverseCallServices">The <see cref="IInitiateReverseCallServices" />.</param>
    /// <param name="unpartitionedFiltersProtocol">The <see cref="IUnpartitionedFiltersProtocol" />.</param>
    /// <param name="partitionedFiltersProtocol">The <see cref="IPartitionedFiltersProtocol" />.</param>
    /// <param name="publicFiltersProtocol">The <see cref="IPublicFiltersProtocol" />.</param>
    /// <param name="createUnpartitionedFilterProcessorFor">The factory to use to create unpartitioned filter processors.</param>
    /// <param name="createPartitionedFilterProcessorFor">The factory to use to create partitioned filter processors.</param>
    /// <param name="createPublicFilterProcessorFor">The factory to use to create public filter processors.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public FiltersService(
        IHostApplicationLifetime hostApplicationLifetime,
        IStreamProcessors streamProcessors,
        IValidateFilterForAllTenants filterForAllTenants,
        ICreateExecutionContexts executionContextCreator,
        IStreamDefinitions streamDefinitions,
        IInitiateReverseCallServices reverseCallServices,
        IUnpartitionedFiltersProtocol unpartitionedFiltersProtocol,
        IPartitionedFiltersProtocol partitionedFiltersProtocol,
        IPublicFiltersProtocol publicFiltersProtocol,
        Func<TenantId, ScopeId, FilterDefinition, UnpartitionedFilterDispatcher, Unpartitioned.FilterProcessor> createUnpartitionedFilterProcessorFor,
        Func<TenantId, ScopeId, FilterDefinition, PartitionedFilterDispatcher, Partitioned.FilterProcessor> createPartitionedFilterProcessorFor,
        Func<TenantId, PublicFilterDefinition, PublicFilterDispatcher, PublicFilterProcessor> createPublicFilterProcessorFor,
        ILogger logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _streamProcessors = streamProcessors;
        _filterForAllTenants = filterForAllTenants;
        _executionContextCreator = executionContextCreator;
        _streamDefinitions = streamDefinitions;
        _reverseCallServices = reverseCallServices;
        _unpartitionedFiltersProtocol = unpartitionedFiltersProtocol;
        _partitionedFiltersProtocol = partitionedFiltersProtocol;
        _publicFiltersProtocol = publicFiltersProtocol;
        _createUnpartitionedFilterProcessorFor = createUnpartitionedFilterProcessorFor;
        _createPartitionedFilterProcessorFor = createPartitionedFilterProcessorFor;
        _createPublicFilterProcessorFor = createPublicFilterProcessorFor;
        _logger = logger;
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

        if (!tryConnect.Success)
        {
            return;
        }
        using var dispatcher = tryConnect.Result.dispatcher;
        var arguments = tryConnect.Result.arguments;
        var createExecutionContext = await CreateExecutionContextOrReject(dispatcher, arguments.ExecutionContext, cts.Token).ConfigureAwait(false);
        if (!createExecutionContext.Success)
        {
            return;
        }

        var executionContext = createExecutionContext.Result;

        if (await RejectIfInvalidFilterId(dispatcher, arguments.Filter, cts.Token).ConfigureAwait(false))
        {
            return;
        }

        var filterDefinition = new FilterDefinition(StreamId.EventLog, arguments.Filter.Value, false);
        await RegisterFilter(
            dispatcher,
            arguments.Scope,
            filterDefinition,
            tenant => _createUnpartitionedFilterProcessorFor(
                tenant,
                arguments.Scope,
                filterDefinition,
                dispatcher),
            executionContext,
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

        if (!tryConnect.Success)
        {
            return;
        }
        using var dispatcher = tryConnect.Result.dispatcher;
        var arguments = tryConnect.Result.arguments;
        var createExecutionContext = await CreateExecutionContextOrReject(dispatcher, arguments.ExecutionContext, cts.Token).ConfigureAwait(false);
        if (!createExecutionContext.Success)
        {
            return;
        }

        var executionContext = createExecutionContext.Result;

        if (await RejectIfInvalidFilterId(dispatcher, arguments.Filter, cts.Token).ConfigureAwait(false))
        {
            return;
        }

        var filterDefinition = new FilterDefinition(StreamId.EventLog, arguments.Filter.Value, true);

        await RegisterFilter(
            dispatcher,
            arguments.Scope,
            filterDefinition, 
            tenant => _createPartitionedFilterProcessorFor(
                tenant,
                arguments.Scope,
                filterDefinition,
                dispatcher),
            executionContext,
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

        if (!tryConnect.Success)
        {
            return;
        }
        using var dispatcher = tryConnect.Result.dispatcher;
        var arguments = tryConnect.Result.arguments;
        var createExecutionContext = await CreateExecutionContextOrReject(dispatcher, arguments.ExecutionContext, cts.Token).ConfigureAwait(false);
        if (!createExecutionContext.Success)
        {
            return;
        }

        var executionContext = createExecutionContext.Result;

        if (await RejectIfInvalidFilterId(dispatcher, arguments.Filter, cts.Token).ConfigureAwait(false))
        {
            return;
        }

        var filterDefinition = new PublicFilterDefinition(StreamId.EventLog, arguments.Filter.Value);
        await RegisterFilter(
            dispatcher,
            ScopeId.Default,
            filterDefinition,
            tenant => _createPublicFilterProcessorFor(
                    tenant,
                    filterDefinition,
                    dispatcher),
            executionContext,
            cts.Token).ConfigureAwait(false);
    }

    Task<Try<ExecutionContext>> CreateExecutionContextOrReject<TClientMessage, TConnectRequest, TResponse>(
        IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
        ExecutionContext requestExecutionContext,
        CancellationToken cancellationToken)
        where TClientMessage : IMessage, new()
        where TConnectRequest : class
        where TResponse : class
        => _executionContextCreator
            .TryCreateUsing(requestExecutionContext)
            .Catch(async exception =>
            {
                _logger.ExecutionContextIsNotValid(exception);
                var failure = new Failure(FiltersFailures.CannotRegisterFilterOnNonWriteableStream, $"Execution context is invalid: {exception.Message}");
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
            });
        

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
        Func<TenantId, IFilterProcessor<TFilterDefinition>> getFilterProcessor,
        ExecutionContext executionContext,
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

        var tryRegisterFilter = TryRegisterStreamProcessor(scopeId, filterDefinition, getFilterProcessor, executionContext, cancellationToken);
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

        var tasks = new TaskGroup(tryStartFilter.Result);
        
        tasks.OnFirstTaskFailure += (_, ex) => _logger.ErrorWhileRunningFilter(ex, filterDefinition.TargetStream, scopeId);
        tasks.OnAllTasksCompleted += () => _logger.FilterStopped(filterDefinition.TargetStream, scopeId);

        await tasks.WaitForAllCancellingOnFirst(linkedTokenSource).ConfigureAwait(false);
    }

    async Task<Try<IEnumerable<Task>>> TryStartFilter<TClientMessage, TConnectRequest, TResponse, TFilterDefinition>(
        IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
        StreamProcessor streamProcessor,
        ScopeId scopeId,
        TFilterDefinition filterDefinition,
        Func<TenantId, IFilterProcessor<TFilterDefinition>> getFilterProcessor,
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
        Func<TenantId, IFilterProcessor<TFilterDefinition>> getFilterProcessor,
        ExecutionContext executionContext,
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
                getFilterProcessor,
                executionContext,
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
        Func<TenantId, IFilterProcessor<TFilterDefinition>> getFilterProcessor,
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

    Task WriteFailedRegistrationResponse<TClientMessage, TConnectRequest, TResponse>(
        IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
        Failure failure,
        CancellationToken cancellationToken)
        where TClientMessage : IMessage, new()
        where TConnectRequest : class
        where TResponse : class => dispatcher.Reject(new FilterRegistrationResponse { Failure = failure }, cancellationToken);
}
