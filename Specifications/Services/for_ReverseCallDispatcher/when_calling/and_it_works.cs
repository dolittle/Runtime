// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_accepting
{
    public class and_it_works : given.a_dispatcher
    {
        static MyConnectResponse connect_response;
        static MyRequest request;
        static MyClientMessage message;
        static Uuid call_id;
        static CancellationTokenSource cts;
        Establish context = () =>
        {
            // setup the accept and while loop for resolving this calls response
            connect_response = new MyConnectResponse();
            client_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            // setup the return message to have the same CallId
            message = new() { Response = new() { Context = new() } };
            // return a null response to keep the while loop going, this gets changed later
            client_stream.SetupGet(_ => _.Current).Returns(message);

            request = new MyRequest();
            server_stream.Setup(_ => _.WriteAsync(Moq.It.IsAny<MyServerMessage>()))
                // access the arguments the call to get the correct protobuf CallId
                .Callback<MyServerMessage>(server_message =>
                {
                    if (server_message.Request.Context.CallId != null)
                    {
                        Console.WriteLine($"WIRING SERVBER MSG: {server_message.Request}");
                        call_id = server_message.Request.Context.CallId;
                        message.Response.Context.CallId = call_id;
                    }
                })
                .Returns(Task.FromResult(true));

            var execution_context = given.execution_contexts.create();
            execution_context_manager.SetupGet(_ => _.Current)
                .Returns(execution_context);
            cts = new();
        };

        static MyResponse response;

        Because of = () =>
        {
            var accept_response = dispatcher.Accept(connect_response, cts.Token);
            var call_response = dispatcher.Call(request, cts.Token);

            // change the stream to now return the client message with the correct CallId
            // client_stream.SetupGet(_ => _.Current).Returns(() => message);
            response = call_response.GetAwaiter().GetResult();
            accept_response.GetAwaiter().GetResult();
            Task.Delay(1000)
                .ContinueWith(t =>
                {
                    cts.Cancel();
                    cts.Dispose();
                });
        };

        It should_write_one_connect_response = () => server_stream
            .Verify(_ =>
                _.WriteAsync(Moq.It.Is<MyServerMessage>(_ =>
                    _.ConnectResponse.Equals(connect_response))), Moq.Times.Once);

        It should_write_one_request = () => server_stream
            .Verify(_ =>
                _.WriteAsync(Moq.It.Is<MyServerMessage>(_ =>
                    _.Request.Equals(request))), Moq.Times.Once);
    }
}
