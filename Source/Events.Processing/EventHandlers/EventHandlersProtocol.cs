// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Services.Contracts;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{

    /// <summary>
    /// Represents the <see cref="IEventHandlersProtocol" />.
    /// </summary>
    public class EventHandlersProtocol : IEventHandlersProtocol
    {
        /// <inheritdoc/>
        public EventHandlerRegistrationArguments ConvertConnectArguments(EventHandlerRegistrationRequest arguments)
            => new(
                arguments.CallContext.ExecutionContext.ToExecutionContext(),
                arguments.EventHandlerId.ToGuid(),
                arguments.Types_.Select(_ => new ArtifactId(_.Id.ToGuid())),
                arguments.Partitioned,
                arguments.ScopeId.ToGuid());

        /// <inheritdoc/>
        public EventHandlerRegistrationResponse CreateFailedConnectResponse(FailureReason failureMessage)
            => new() { Failure = new Dolittle.Protobuf.Contracts.Failure { Id = EventHandlersFailures.FailedToRegisterEventHandler.Value.ToProtobuf(), Reason = failureMessage } };

        /// <inheritdoc/>
        public ReverseCallArgumentsContext GetArgumentsContext(EventHandlerRegistrationRequest message)
            => message.CallContext;

        /// <inheritdoc/>
        public EventHandlerRegistrationRequest GetConnectArguments(EventHandlerClientToRuntimeMessage message)
            => message.RegistrationRequest;

        /// <inheritdoc/>
        public Pong GetPong(EventHandlerClientToRuntimeMessage message)
            => message.Pong;

        /// <inheritdoc/>
        public EventHandlerResponse GetResponse(EventHandlerClientToRuntimeMessage message)
            => message.HandleResult;

        /// <inheritdoc/>
        public ReverseCallResponseContext GetResponseContext(EventHandlerResponse message)
            => message.CallContext;

        /// <inheritdoc/>
        public void SetConnectResponse(EventHandlerRegistrationResponse arguments, EventHandlerRuntimeToClientMessage message)
            => message.RegistrationResponse = arguments;

        /// <inheritdoc/>
        public void SetPing(EventHandlerRuntimeToClientMessage message, Ping ping)
            => message.Ping = ping;

        /// <inheritdoc/>
        public void SetRequest(HandleEventRequest request, EventHandlerRuntimeToClientMessage message)
            => message.HandleRequest = request;

        /// <inheritdoc/>
        public void SetRequestContext(ReverseCallRequestContext context, HandleEventRequest request)
            => request.CallContext = context;

        /// <inheritdoc/>
        public ConnectArgumentsValidationResult ValidateConnectArguments(EventHandlerRegistrationArguments arguments)
            => ConnectArgumentsValidationResult.Ok;
    }
}
