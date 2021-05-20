// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading.Tasks;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Services;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using static Dolittle.Runtime.Embeddings.Contracts.Embeddings;
using Dolittle.Runtime.Tenancy;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Protobuf;
using System.Threading;
using Dolittle.Runtime.Rudimentary;
using System.Runtime.ExceptionServices;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing
{
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
        readonly ILogger _logger;
        readonly IHostApplicationLifetime _hostApplicationLifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingsService"/> class.
        /// </summary>
        /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="reverseCallServices">The <see cref="IInitiateReverseCallServices" />.</param>
        /// <param name="protocol">The <see cref="IProjectionsProtocol" />.</param>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="embeddingProcessorFactory">The <see cref="IEmbeddingProcessorFactory" />.</param>
        /// <param name="embeddingProcessors">The <see cref="IEmbeddingProcessors" />.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public EmbeddingsService(
            IHostApplicationLifetime hostApplicationLifetime,
            IExecutionContextManager executionContextManager,
            IInitiateReverseCallServices reverseCallServices,
            IEmbeddingsProtocol protocol,
            IEmbeddingProcessorFactory embeddingProcessorFactory,
            IEmbeddingProcessors embeddingProcessors,
            ILogger<EmbeddingsService> logger)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _executionContextManager = executionContextManager;
            _reverseCallServices = reverseCallServices;
            _protocol = protocol;
            _embeddingProcessorFactory = embeddingProcessorFactory;
            _embeddingProcessors = embeddingProcessors;
            _logger = logger;
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

            if (_embeddingProcessors.HasEmbeddingProcessors(arguments.EmbeddingId))
            {
                await dispatcher.Reject(
                    _protocol.CreateFailedConnectResponse($"Failed to register Embedding: {arguments.EmbeddingId.Value}. Embedding already registered with the same id"),
                    cts.Token).ConfigureAwait(false);
                return;
            }

            var dispatcherTask = dispatcher.Accept(new EmbeddingRegistrationResponse(), cts.Token);
            var processorTask = _embeddingProcessors.TryStartEmbeddingProcessorForAllTenants(
                arguments.EmbeddingId,
                tenant => _embeddingProcessorFactory.Create(tenant, arguments.EmbeddingId),
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
    }
}
