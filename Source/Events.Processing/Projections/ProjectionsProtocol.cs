// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents the <see cref="IProjectionsProtocol" />.
/// </summary>
public class ProjectionsProtocol : IProjectionsProtocol
{
    readonly IConvertProjectionDefinitions _converter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsProtocol"/> class.
    /// </summary>
    /// <param name="converter">The converter to use to convert projection definition fields.</param>
    public ProjectionsProtocol(IConvertProjectionDefinitions converter)
    {
        _converter = converter;
    }

    /// <inheritdoc/>
    public ProjectionRegistrationArguments ConvertConnectArguments(ProjectionRegistrationRequest arguments)
        => arguments.HasAlias switch
        {
            true => new ProjectionRegistrationArguments(
                arguments.CallContext.ExecutionContext.ToExecutionContext(),
                ConvertProjectionDefinition(arguments),
                arguments.Alias),
            false => new ProjectionRegistrationArguments(
                arguments.CallContext.ExecutionContext.ToExecutionContext(),
                ConvertProjectionDefinition(arguments)),
        };

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
        foreach (var eventType in arguments.ProjectionDefinition.Events.GroupBy(_ => _.EventType))
        {
            if (eventType.Count() > 1)
            {
                return ConnectArgumentsValidationResult.Failed($"Event {eventType.Key.Value} was specified more than once");
            }
        }
        return ConnectArgumentsValidationResult.Ok;
    }

    ProjectionDefinition ConvertProjectionDefinition(ProjectionRegistrationRequest arguments)
        => new(
            arguments.ProjectionId.ToGuid(),
            arguments.ScopeId.ToGuid(),
            _converter.ToRuntimeEventSelectors(arguments.Events),
            arguments.InitialState,
            _converter.ToRuntimeCopySpecification(arguments.Copies));
}
