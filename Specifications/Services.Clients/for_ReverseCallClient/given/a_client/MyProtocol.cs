// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.Services.Contracts;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client
{
    public class MyProtocol : IReverseCallClientProtocol<MyClient, MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>
    {
        public AsyncDuplexStreamingCall<MyClientMessage, MyServerMessage> Call(MyClient client, CallOptions callOptions)
            => client.Method(callOptions);

        public MyConnectResponse GetConnectResponse(MyServerMessage message)
            => message.ConnectResponse;

        public Failure GetFailureFromConnectResponse(MyConnectResponse response)
            => response.Failure;

        public Ping GetPing(MyServerMessage message)
            => message.Ping;

        public MyRequest GetRequest(MyServerMessage message)
            => message.Request;

        public ReverseCallRequestContext GetRequestContext(MyRequest message)
            => message.Context;

        public void SetConnectArguments(MyConnectArguments arguments, MyClientMessage message)
            => message.Arguments = arguments;

        public void SetConnectArgumentsContext(ReverseCallArgumentsContext context, MyConnectArguments arguments)
            => arguments.Context = context;

        public void SetPong(Pong pong, MyClientMessage message)
            => message.Pong = pong;

        public void SetResponse(MyResponse response, MyClientMessage message)
            => message.Response = response;

        public void SetResponseContext(ReverseCallResponseContext context, MyResponse response)
            => response.Context = context;
    }
}
