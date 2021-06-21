// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents the <see cref="IEmbeddingsProtocol" />.
    /// </summary>
    public class EmbeddingsProtocol : IEmbeddingsProtocol
    {
        /// <inheritdoc/>
        public EmbeddingRegistrationArguments ConvertConnectArguments(EmbeddingRegistrationRequest arguments)
            => new(
                arguments.CallContext.ExecutionContext.ToExecutionContext(),
                arguments.EmbeddingId.ToGuid(),
                arguments.Events.Select(_ => _.ToArtifact()),
                arguments.InitialState);

        /// <inheritdoc/>
        public EmbeddingRegistrationResponse CreateFailedConnectResponse(FailureReason failureMessage)
            => new() { Failure = new Dolittle.Protobuf.Contracts.Failure { Id = EmbeddingFailures.FailedToRegisterEmbedding.Value.ToProtobuf(), Reason = failureMessage } };

        /// <inheritdoc/>
        public ReverseCallArgumentsContext GetArgumentsContext(EmbeddingRegistrationRequest message)
            => message.CallContext;

        /// <inheritdoc/>
        public EmbeddingRegistrationRequest GetConnectArguments(EmbeddingClientToRuntimeMessage message)
            => message.RegistrationRequest;

        /// <inheritdoc/>
        public Pong GetPong(EmbeddingClientToRuntimeMessage message)
            => message.Pong;

        /// <inheritdoc/>
        public EmbeddingResponse GetResponse(EmbeddingClientToRuntimeMessage message)
            => message.HandleResult;

        /// <inheritdoc/>
        public ReverseCallResponseContext GetResponseContext(EmbeddingResponse message)
            => message.CallContext;

        /// <inheritdoc/>
        public void SetConnectResponse(EmbeddingRegistrationResponse arguments, EmbeddingRuntimeToClientMessage message)
            => message.RegistrationResponse = arguments;

        /// <inheritdoc/>
        public void SetPing(EmbeddingRuntimeToClientMessage message, Ping ping)
            => message.Ping = ping;

        /// <inheritdoc/>
        public void SetRequest(EmbeddingRequest request, EmbeddingRuntimeToClientMessage message)
            => message.HandleRequest = request;

        /// <inheritdoc/>
        public void SetRequestContext(ReverseCallRequestContext context, EmbeddingRequest request)
            => request.CallContext = context;

        /// <inheritdoc/>
        public ConnectArgumentsValidationResult ValidateConnectArguments(EmbeddingRegistrationArguments arguments)
        {
            foreach (var eventTypeGroup in arguments.Events.GroupBy(_ => _.Id))
            {
                if (eventTypeGroup.Count() > 1)
                {
                    return ConnectArgumentsValidationResult.Failed($"Event {eventTypeGroup.Key.Value} was specified more than once");
                }
            }
            return ConnectArgumentsValidationResult.Ok;
        }
    }
}
