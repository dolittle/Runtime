// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Services.Contracts;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents a base implementation for <see cref="IFiltersProtocol{TFilterClientMessage, TFilterRegistrationRequest, TFilterResponse}" />.
/// </summary>
/// <typeparam name="TFilterClientMessage">Type of the messages that is sent from the client to the server.</typeparam>
/// <typeparam name="TFilterRegistrationRequest">Type of the arguments that are sent along with the initial Connect call.</typeparam>
/// <typeparam name="TFilterResponse">Type of the responses received from the client.</typeparam>
public abstract class FiltersProtocol<TFilterClientMessage, TFilterRegistrationRequest, TFilterResponse, TRuntimeRegistrationArguments> : IFiltersProtocol<TFilterClientMessage, TFilterRegistrationRequest, TFilterResponse, TRuntimeRegistrationArguments>
    where TFilterClientMessage : IMessage, new()
    where TFilterRegistrationRequest : class
    where TFilterResponse : class
    where TRuntimeRegistrationArguments : class
{
    /// <inheritdoc/>
    public abstract TRuntimeRegistrationArguments ConvertConnectArguments(TFilterRegistrationRequest arguments);

    /// <inheritdoc/>
    public abstract ReverseCallArgumentsContext GetArgumentsContext(TFilterRegistrationRequest message);

    /// <inheritdoc/>
    public abstract TFilterRegistrationRequest GetConnectArguments(TFilterClientMessage message);

    /// <inheritdoc/>
    public abstract Pong GetPong(TFilterClientMessage message);

    /// <inheritdoc/>
    public abstract TFilterResponse GetResponse(TFilterClientMessage message);

    /// <inheritdoc/>
    public abstract ReverseCallResponseContext GetResponseContext(TFilterResponse message);

    /// <inheritdoc/>
    public void SetConnectResponse(FilterRegistrationResponse arguments, FilterRuntimeToClientMessage message)
        => message.RegistrationResponse = arguments;

    /// <inheritdoc/>
    public void SetPing(FilterRuntimeToClientMessage message, Ping ping)
        => message.Ping = ping;

    /// <inheritdoc/>
    public void SetRequest(FilterEventRequest request, FilterRuntimeToClientMessage message)
        => message.FilterRequest = request;

    /// <inheritdoc/>
    public void SetRequestContext(ReverseCallRequestContext context, FilterEventRequest request)
        => request.CallContext = context;

    /// <inheritdoc/>
    public FilterRegistrationResponse CreateFailedConnectResponse(FailureReason failureMessage)
        => new() { Failure = new Dolittle.Protobuf.Contracts.Failure { Id = FiltersFailures.FailedToRegisterFilter.Value.ToProtobuf(), Reason = failureMessage } };

    /// <inheritdoc/>
    public ConnectArgumentsValidationResult ValidateConnectArguments(TRuntimeRegistrationArguments arguments)
        => ConnectArgumentsValidationResult.Ok;
}