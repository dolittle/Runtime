// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using RuntimeProjectionEventSelector = Dolittle.Runtime.Projections.Store.Definition.ProjectionEventSelector;
using RuntimeProjectEventKeySelectorType = Dolittle.Runtime.Projections.Store.Definition.ProjectEventKeySelectorType;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents the <see cref="IProjectionsProtocol" />.
    /// </summary>
    public class ProjectionsProtocol : IProjectionsProtocol
    {
        /// <inheritdoc/>
        public ProjectionRegistrationArguments ConvertConnectArguments(ProjectionRegistrationRequest arguments)
            => new(
                arguments.CallContext.ExecutionContext.ToExecutionContext(),
                new ProjectionDefinition(
                    arguments.ProjectionId.ToGuid(),
                    arguments.ScopeId.ToGuid(),
                    arguments.Events.Select(_ => new RuntimeProjectionEventSelector(_.EventType.ToArtifact(), (RuntimeProjectEventKeySelectorType)_.KeySelector.Type, _.KeySelector.Expression)),
                    arguments.InitialState
                ));

        /// <inheritdoc/>
        public ProjectionRegistrationResponse CreateFailedConnectResponse(FailureReason failureMessage)
            => new() { Failure = new Dolittle.Protobuf.Contracts.Failure { Id = ProjectionFailures.FailedToRegisterProjection.Value.ToProtobuf(), Reason = failureMessage } };

        /// <inheritdoc/>
        public ReverseCallArgumentsContext GetArgumentsContext(ProjectionRegistrationRequest message)
            => message.CallContext;

        /// <inheritdoc/>
        public ProjectionRegistrationRequest GetConnectArguments(ProjectionClientToRuntimeMessage message)
            => message.RegistrationRequest;

        /// <inheritdoc/>
        public Pong GetPong(ProjectionClientToRuntimeMessage message)
            => message.Pong;

        /// <inheritdoc/>
        public ProjectionResponse GetResponse(ProjectionClientToRuntimeMessage message)
            => message.HandleResult;

        /// <inheritdoc/>
        public ReverseCallResponseContext GetResponseContext(ProjectionResponse message)
            => message.CallContext;

        /// <inheritdoc/>
        public void SetConnectResponse(ProjectionRegistrationResponse arguments, ProjectionRuntimeToClientMessage message)
            => message.RegistrationResponse = arguments;

        /// <inheritdoc/>
        public void SetPing(ProjectionRuntimeToClientMessage message, Ping ping)
            => message.Ping = ping;

        /// <inheritdoc/>
        public void SetRequest(ProjectionRequest request, ProjectionRuntimeToClientMessage message)
            => message.HandleRequest = request;

        /// <inheritdoc/>
        public void SetRequestContext(ReverseCallRequestContext context, ProjectionRequest request)
            => request.CallContext = context;

        /// <inheritdoc/>
        public ConnectArgumentsValidationResult ValidateConnectArguments(ProjectionRegistrationArguments arguments)
        {
            foreach (var eventType in arguments.ProjectionDefinition.Events.GroupBy(_ => _.EventType.Id))
            {
                if (eventType.Count() > 1)
                {
                    return ConnectArgumentsValidationResult.Failed($"Event {eventType.Key.Value} was specified more than once");
                }
            }
            return ConnectArgumentsValidationResult.Ok;
        }
    }
}
