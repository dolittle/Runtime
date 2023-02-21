// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Services.Contracts;

namespace Integration.Tests.Services.given;

record RegistrationArguments(bool Valid);

class basic_with_ping_pong_reverse_call_service_protocol : IReverseCallServiceProtocol<ClientToRuntimeMessage, RuntimeToClientMessage, RegistrationRequest, RegistrationResponse, Request, Response, RegistrationArguments>
{
    public RegistrationRequest GetConnectArguments(ClientToRuntimeMessage message)
        => message.RegistrationRequest;

    public void SetConnectResponse(RegistrationResponse arguments, RuntimeToClientMessage message)
    {
        message.RegistrationResponse = arguments;
    }

    public void SetRequest(Request request, RuntimeToClientMessage message)
    {
        message.HandleRequest = request;
    }

    public Response GetResponse(ClientToRuntimeMessage message)
        => message.Response;

    public ReverseCallArgumentsContext GetArgumentsContext(RegistrationRequest message)
        => message.CallContext;

    public void SetRequestContext(ReverseCallRequestContext context, Request request)
    {
        request.CallContext = context;
    }

    public ReverseCallResponseContext GetResponseContext(Response message)
        => message.CallContext;

    public void SetPing(RuntimeToClientMessage message, Ping ping)
    {
        message.Ping = ping;
    }

    public Pong GetPong(ClientToRuntimeMessage message)
        => message.Pong;

    public RegistrationResponse CreateFailedConnectResponse(FailureReason failureMessage)
        => new(){Failed = true};

    public RegistrationArguments ConvertConnectArguments(RegistrationRequest arguments)
        => new(arguments.IsValid);

    public ConnectArgumentsValidationResult ValidateConnectArguments(RegistrationArguments arguments)
        => arguments.Valid ? ConnectArgumentsValidationResult.Ok : ConnectArgumentsValidationResult.Failed("Not valid");
}