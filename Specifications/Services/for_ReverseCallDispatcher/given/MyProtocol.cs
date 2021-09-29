// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.given
{
    public class MyProtocol : IConvertReverseCallMessages<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>
    {
        public MyConnectResponse CreateFailedConnectResponse(FailureReason failureMessage)
            => new();

        public ReverseCallArgumentsContext GetArgumentsContext(MyConnectArguments message)
            => message.Context;

        public MyConnectArguments GetConnectArguments(MyClientMessage message)
            => message.Arguments;

        public Pong GetPong(MyClientMessage message)
            => message.Pong;

        public MyResponse GetResponse(MyClientMessage message)
            => message.Response;

        public ReverseCallResponseContext GetResponseContext(MyResponse message)
            => message.Context;

        public void SetConnectResponse(MyConnectResponse arguments, MyServerMessage message)
            => message.ConnectResponse = arguments;

        public void SetPing(MyServerMessage message, Ping ping)
            => message.Ping = ping;

        public void SetRequest(MyRequest request, MyServerMessage message)
            => message.Request = request;

        public void SetRequestContext(ReverseCallRequestContext context, MyRequest request)
            => request.Context = context;
    }
}
