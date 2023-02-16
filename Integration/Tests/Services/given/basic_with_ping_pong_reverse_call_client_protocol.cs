// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Clients;
using Dolittle.Services.Contracts;
using Grpc.Core;
using Failure = Dolittle.Protobuf.Contracts.Failure;

namespace Integration.Tests.Services.given;

class basic_with_ping_pong_reverse_call_client_protocol : IReverseCallClientProtocol<BasicReverseCallService.BasicReverseCallServiceClient, ClientToRuntimeMessage, RuntimeToClientMessage, RegistrationRequest, RegistrationResponse, Request, Response>
{
    public AsyncDuplexStreamingCall<ClientToRuntimeMessage, RuntimeToClientMessage> Call(BasicReverseCallService.BasicReverseCallServiceClient client, CallOptions callOptions)
        => client.Connect(callOptions);

    public void SetConnectArgumentsContext(ReverseCallArgumentsContext context, RegistrationRequest arguments)
    {
        arguments.CallContext = context;
    }

    public void SetConnectArguments(RegistrationRequest arguments, ClientToRuntimeMessage message)
    {
        message.RegistrationRequest = arguments;
    }

    public RegistrationResponse GetConnectResponse(RuntimeToClientMessage message)
        => message.RegistrationResponse;

    public Failure GetFailureFromConnectResponse(RegistrationResponse response)
        => response.Failed ? new Failure
        {
            Id = Guid.NewGuid().ToProtobuf(),
            Reason = "Not valid"
        }
            : null;

    public Ping GetPing(RuntimeToClientMessage message)
        => message.Ping;

    public void SetPong(Pong pong, ClientToRuntimeMessage message)
    {
        message.Pong = pong;
    }

    public Request GetRequest(RuntimeToClientMessage message)
        => message.HandleRequest;

    public ReverseCallRequestContext GetRequestContext(Request message)
        => message.CallContext;

    public void SetResponseContext(ReverseCallResponseContext context, Response response)
    {
        response.CallContext = context;
    }

    public void SetResponse(Response response, ClientToRuntimeMessage message)
    {
        message.Response = response;
    }
}