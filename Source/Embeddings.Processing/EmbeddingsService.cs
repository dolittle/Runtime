// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Services;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using static Dolittle.Runtime.Embeddings.Contracts.Embeddings;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Protobuf;
using System.Threading;
using Dolittle.Runtime.Rudimentary;
using System.Runtime.ExceptionServices;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.Definition;
using System.Linq;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents the implementation of <see cref="EmbeddingsBase"/>.
/// </summary>
public class EmbeddingsService : EmbeddingsBase
{
    readonly IExecutionContextManager _executionContextManager;
    readonly IInitiateReverseCallServices _reverseCallServices;
    readonly IEmbeddingsProtocol _protocol;
    readonly IEmbeddingProcessorFactory _embeddingProcessorFactory;
    readonly IEmbeddingProcessors _embeddingProcessors;
    readonly IEmbeddingRequestFactory _embeddingRequestFactory;
    readonly ICompareEmbeddingDefinitionsForAllTenants _embeddingDefinitionComparer;
    readonly IPersistEmbeddingDefinitionForAllTenants _embeddingDefinitionPersister;
    readonly ILogger _logger;
    readonly ILoggerFactory _loggerFactory;
    readonly IHostApplicationLifetime _hostApplicationLifetime;

    /// <summary>
    /// Initializes an instance of the <see cref="EmbeddingsService" /> class.
    /// </summary>
    /// <param name="hostApplicationLifetime"></param>
    /// <param name="executionContextManager"></param>
    /// <param name="reverseCallServices"></param>
    /// <param name="protocol"></param>
    /// <param name="embeddingProcessorFactory"></param>
    /// <param name="embeddingProcessors"></param>
    /// <param name="embeddingRequestFactory"></param>
    /// <param name="embeddingDefinitionComparer"></param>
    /// <param name="embeddingDefinitionPersister"></param>
    /// <param name="logger"></param>
    public EmbeddingsService(
        IHostApplicationLifetime hostApplicationLifetime,
        IExecutionContextManager executionContextManager,
        IInitiateReverseCallServices reverseCallServices,
        IEmbeddingsProtocol protocol,
        IEmbeddingProcessorFactory embeddingProcessorFactory,
        IEmbeddingProcessors embeddingProcessors,
        IEmbeddingRequestFactory embeddingRequestFactory,
        ICompareEmbeddingDefinitionsForAllTenants embeddingDefinitionComparer,
        IPersistEmbeddingDefinitionForAllTenants embeddingDefinitionPersister,
        ILogger<EmbeddingsService> logger,
        ILoggerFactory loggerFactory)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _executionContextManager = executionContextManager;
        _reverseCallServices = reverseCallServices;
        _protocol = protocol;
        _embeddingProcessorFactory = embeddingProcessorFactory;
        _embeddingProcessors = embeddingProcessors;
        _embeddingRequestFactory = embeddingRequestFactory;
        _embeddingDefinitionComparer = embeddingDefinitionComparer;
        _embeddingDefinitionPersister = embeddingDefinitionPersister;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public override async Task Connect(
        IAsyncStreamReader<EmbeddingClientToRuntimeMessage> runtimeStream,
        IServerStreamWriter<EmbeddingRuntimeToClientMessage> clientStream,
        ServerCallContext context)
    {
        _logger.LogDebug("Connecting Embeddings");
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
        var connection = await _reverseCallServices.Connect(runtimeStream, clientStream, context, _protocol, context.CancellationToken).ConfigureAwait(false);
        if (!connection.Success)
        {
            return;
        }
        var (dispatcher, arguments) = connection.Result;
        _executionContextManager.CurrentFor(arguments.ExecutionContext);

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
            tenant => _embeddingProcessorFactory.Create(
                tenant,
                arguments.Definition.Embedding,
                new Embedding(arguments.Definition.Embedding, dispatcher, _embeddingRequestFactory, _loggerFactory.CreateLogger<Embedding>()),
                arguments.Definition.InititalState),
            cts.Token);
        var tasks = new[] { dispatcherTask, processorTask };

        try
        {
            await Task.WhenAny(tasks).ConfigureAwait(false);

            if (tasks.TryGetFirstInnerMostException(out var ex))
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }
        finally
        {
            cts.Cancel();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
        _executionContextManager.CurrentFor(request.CallContext.ExecutionContext);
        if (!TryGetRegisteredEmbeddingProcessorForTenant(_executionContextManager.Current.Tenant, request.EmbeddingId.ToGuid(), out var processor, out var failure))
        {
            return new UpdateResponse
            {
                Failure = failure
            };
        }
        var newState = await processor.Update(request.Key, request.State, cts.Token).ConfigureAwait(false);
        if (!newState.Success)
        {
            return new UpdateResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                {
                    Id = EmbeddingFailures.FailedToUpdateEmbedding.ToProtobuf(),
                    Reason = $"Failed to update embedding {request.EmbeddingId.ToGuid()} for tenant {_executionContextManager.Current.Tenant.Value} with key {request.Key}. {newState.Exception.Message}"
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
        _executionContextManager.CurrentFor(request.CallContext.ExecutionContext);
        if (!TryGetRegisteredEmbeddingProcessorForTenant(_executionContextManager.Current.Tenant, request.EmbeddingId.ToGuid(), out var processor, out var failure))
        {
            return new DeleteResponse
            {
                Failure = failure
            };
        }
        var deleteEmbedding = await processor.Delete(request.Key, cts.Token).ConfigureAwait(false);
        if (!deleteEmbedding.Success)
        {
            return new DeleteResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                {
                    Id = EmbeddingFailures.FailedToDeleteEmbedding.ToProtobuf(),
                    Reason = $"Failed to delete embedding {request.EmbeddingId.ToGuid()} for tenant {_executionContextManager.Current.Tenant.Value}. {deleteEmbedding.Exception.Message}"
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

        if (!tenantsAndComparisonResult.Values.Any(_ => !_.Succeeded))
        {
            return false;
        }
        var unsuccessfulComparisons = tenantsAndComparisonResult
            .Where(_ => !_.Value.Succeeded)
            .Select(_ =>
            {
                return (_.Key, _.Value);
            });
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