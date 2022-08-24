// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Services;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Protobuf;
using System.Threading;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.Definition;
using System.Linq;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Services.Hosting;
using static Dolittle.Runtime.Embeddings.Contracts.Embeddings;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents the implementation of <see cref="EmbeddingsBase"/>.
/// </summary>
[PrivateService]
public class EmbeddingsService : EmbeddingsBase
{
    readonly IInitiateReverseCallServices _reverseCallServices;
    readonly IEmbeddingsProtocol _protocol;
    readonly Func<TenantId, IEmbeddingProcessorFactory> _getEmbeddingProcessorFactoryFor;
    readonly IEmbeddingProcessors _embeddingProcessors;
    readonly ICompareEmbeddingDefinitionsForAllTenants _embeddingDefinitionComparer;
    readonly IPersistEmbeddingDefinitionForAllTenants _embeddingDefinitionPersister;
    readonly IEmbeddingRequestFactory _embeddingRequestFactory;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger _logger;
    readonly IHostApplicationLifetime _hostApplicationLifetime;
    readonly ICreateExecutionContexts _executionContextCreator;

    /// <summary>
    /// Initializes an instance of the <see cref="EmbeddingsService" /> class.
    /// </summary>
    /// <param name="hostApplicationLifetime"></param>
    /// <param name="executionContextCreator"></param>
    /// <param name="reverseCallServices"></param>
    /// <param name="protocol"></param>
    /// <param name="getEmbeddingProcessorFactoryFor"></param>
    /// <param name="embeddingProcessors"></param>
    /// <param name="embeddingRequestFactory"></param>
    /// <param name="embeddingDefinitionComparer"></param>
    /// <param name="embeddingDefinitionPersister"></param>
    /// <param name="logger"></param>
    public EmbeddingsService(
        IHostApplicationLifetime hostApplicationLifetime,
        ICreateExecutionContexts executionContextCreator,
        IInitiateReverseCallServices reverseCallServices,
        IEmbeddingsProtocol protocol,
        Func<TenantId, IEmbeddingProcessorFactory> getEmbeddingProcessorFactoryFor,
        IEmbeddingProcessors embeddingProcessors,
        ICompareEmbeddingDefinitionsForAllTenants embeddingDefinitionComparer,
        IPersistEmbeddingDefinitionForAllTenants embeddingDefinitionPersister,
        IEmbeddingRequestFactory embeddingRequestFactory,
        ILoggerFactory loggerFactory)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _executionContextCreator = executionContextCreator;
        _reverseCallServices = reverseCallServices;
        _protocol = protocol;
        _getEmbeddingProcessorFactoryFor = getEmbeddingProcessorFactoryFor;
        _embeddingProcessors = embeddingProcessors;
        _embeddingDefinitionComparer = embeddingDefinitionComparer;
        _embeddingDefinitionPersister = embeddingDefinitionPersister;
        _embeddingRequestFactory = embeddingRequestFactory;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<EmbeddingsService>();
    }

    /// <inheritdoc/>
    public override async Task Connect(
        IAsyncStreamReader<EmbeddingClientToRuntimeMessage> runtimeStream,
        IServerStreamWriter<EmbeddingRuntimeToClientMessage> clientStream,
        ServerCallContext context)
    {
        // TODO: It seems like things are not properly unregistered on exceptions?
        // TODO: I tested this out and while making the DI container work, it kept failing and telling me that the projection was already registered on the second attempt.
        
        Log.ConnectingEmbeddings(_logger);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
        var connection = await _reverseCallServices.Connect(runtimeStream, clientStream, context, _protocol, context.CancellationToken).ConfigureAwait(false);
        if (!connection.Success)
        {
            return;
        }
        using var dispatcher = connection.Result.dispatcher;
        var arguments = connection.Result.arguments;

        var tryCreateExecutionContext = _executionContextCreator.TryCreateUsing(arguments.ExecutionContext);
        if (!tryCreateExecutionContext.Success)
        {
            await dispatcher.Reject(
                _protocol.CreateFailedConnectResponse($"Failed to register embedding because the execution context is invalid: ${tryCreateExecutionContext.Exception.Message}"),
                cts.Token).ConfigureAwait(false);
            return;
        }

        var executionContext = tryCreateExecutionContext.Result; // TODO: Use this

        if (_embeddingProcessors.HasEmbeddingProcessors(arguments.Definition.Embedding))
        {
            await dispatcher.Reject(
                _protocol.CreateFailedConnectResponse($"Failed to register Embedding: {arguments.Definition.Embedding.Value}. Embedding already registered with the same id"),
                cts.Token).ConfigureAwait(false);
            return;
        }

        if (await RejectIfInvalidDefinition(arguments.Definition, dispatcher, cts.Token).ConfigureAwait(false))
        {
            return;
        }
        var persistDefinition = await _embeddingDefinitionPersister.TryPersist(arguments.Definition, cts.Token).ConfigureAwait(false);
        if (!persistDefinition.Success)
        {
            await dispatcher.Reject(
                _protocol.CreateFailedConnectResponse($"Failed to register Embedding: {arguments.Definition.Embedding.Value}. Failed to persist embedding definition. {persistDefinition.Exception.Message}"),
                cts.Token).ConfigureAwait(false);
            return;
        }

        var dispatcherTask = dispatcher.Accept(new EmbeddingRegistrationResponse(), cts.Token);
        var processorTask = _embeddingProcessors.TryStartEmbeddingProcessorForAllTenants(
            arguments.Definition.Embedding,
            tenant => _getEmbeddingProcessorFactoryFor(tenant)
                .Create(
                    arguments.Definition.Embedding,
                    new Embedding(
                        arguments.Definition.Embedding,
                        dispatcher,
                        _embeddingRequestFactory,
                        _loggerFactory.CreateLogger<Embedding>()),
                    arguments.Definition.InititalState,
                    executionContext),
            cts.Token);


        var tasks = new TaskGroup(dispatcherTask, processorTask);
        await tasks.WaitForAllCancellingOnFirst(cts).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
        
        var tryCreateExecutionContext = _executionContextCreator.TryCreateUsing(request.CallContext.ExecutionContext);
        if (!tryCreateExecutionContext.Success)
        {
            return new UpdateResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                {
                    Id = EmbeddingFailures.FailedToUpdateEmbedding.ToProtobuf(),
                    Reason = $"Failed to update embedding {request.EmbeddingId.ToGuid()} because the execution context was invalid: {tryCreateExecutionContext.Exception.Message}"
                }
            };
        }

        var executionContext = tryCreateExecutionContext.Result;
        if (!TryGetRegisteredEmbeddingProcessorForTenant(executionContext.Tenant, request.EmbeddingId.ToGuid(), out var processor, out var failure))
        {
            return new UpdateResponse
            {
                Failure = failure
            };
        }
        var newState = await processor.Update(request.Key, request.State, executionContext, cts.Token).ConfigureAwait(false);
        if (!newState.Success)
        {
            return new UpdateResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                {
                    Id = EmbeddingFailures.FailedToUpdateEmbedding.ToProtobuf(),
                    Reason = $"Failed to update embedding {request.EmbeddingId.ToGuid()} for tenant {executionContext.Tenant} with key {request.Key}. {newState.Exception.Message}"
                }
            };
        }
        return new UpdateResponse
        {
            State = new Projections.Contracts.ProjectionCurrentState
            {
                Type = Projections.Contracts.ProjectionCurrentStateType.Persisted,
                Key = request.Key,
                State = newState.Result
            }
        };
    }

    public override async Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
        
        var tryCreateExecutionContext = _executionContextCreator.TryCreateUsing(request.CallContext.ExecutionContext);
        if (!tryCreateExecutionContext.Success)
        {
            return new DeleteResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                {
                    Id = EmbeddingFailures.FailedToUpdateEmbedding.ToProtobuf(),
                    Reason = $"Failed to delete embedding {request.EmbeddingId.ToGuid()} because the execution context was invalid: {tryCreateExecutionContext.Exception.Message}"
                }
            };
        }

        var executionContext = tryCreateExecutionContext.Result;
        if (!TryGetRegisteredEmbeddingProcessorForTenant(executionContext.Tenant, request.EmbeddingId.ToGuid(), out var processor, out var failure))
        {
            return new DeleteResponse
            {
                Failure = failure
            };
        }
        var deleteEmbedding = await processor.Delete(request.Key, executionContext, cts.Token).ConfigureAwait(false);
        if (!deleteEmbedding.Success)
        {
            return new DeleteResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                {
                    Id = EmbeddingFailures.FailedToDeleteEmbedding.ToProtobuf(),
                    Reason = $"Failed to delete embedding {request.EmbeddingId.ToGuid()} for tenant {executionContext.Tenant}. {deleteEmbedding.Exception.Message}"
                }
            };
        }

        return new DeleteResponse();
    }

    bool TryGetRegisteredEmbeddingProcessorForTenant(TenantId tenant, EmbeddingId embedding, out IEmbeddingProcessor processor, out Failure failure)
    {
        failure = default;
        if (!_embeddingProcessors.TryGetEmbeddingProcessorFor(tenant, embedding, out processor))
        {
            failure = new Dolittle.Protobuf.Contracts.Failure
            {
                Id = EmbeddingFailures.NoEmbeddingRegisteredForTenant.ToProtobuf(),
                Reason = $"No embedding with id {embedding.Value} registered for tenant {tenant.Value}"
            };
            return false;
        }
        return true;
    }

    async Task<bool> RejectIfInvalidDefinition(
        EmbeddingDefinition definition,
        IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse> dispatcher,
        CancellationToken cancellationToken)
    {
        _logger.ComparingEmbeddingDefinition(definition);
        var tenantsAndComparisonResult = await _embeddingDefinitionComparer.DiffersFromPersisted(
            new EmbeddingDefinition(
                definition.Embedding,
                definition.Events,
                definition.InititalState),
            cancellationToken).ConfigureAwait(false);

        if (tenantsAndComparisonResult.Values.All(_ => _.Succeeded))
        {
            return false;
        }
        
        var unsuccessfulComparisons = tenantsAndComparisonResult
            .Where(_ => !_.Value.Succeeded)
            .Select(_ => (_.Key, _.Value));
        
        _logger.InvalidEmbeddingDefinition(definition, unsuccessfulComparisons);

        await dispatcher.Reject(CreateInvalidValidationResponse(
                unsuccessfulComparisons,
                definition.Embedding),
            cancellationToken).ConfigureAwait(false);


        return true;
    }
    EmbeddingRegistrationResponse CreateInvalidValidationResponse(
        System.Collections.Generic.IEnumerable<(TenantId Key, EmbeddingDefinitionComparisonResult Value)> unsuccessfulComparisons,
        EmbeddingId embedding)
    {
        var (_, result) = unsuccessfulComparisons.First();
        return _protocol.CreateFailedConnectResponse($"Failed to register Embedding: {embedding.Value} for tenant. {result.FailureReason.Value}");
    }
}
