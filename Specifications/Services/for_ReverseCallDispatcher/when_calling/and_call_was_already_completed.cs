// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_calling
{
    public class and_call_was_already_completed : given.a_response
    {
        static MyRequest request;
        static MyConnectResponse connect_response;

        Establish context = () =>
        {
            request = new MyRequest();
            connect_response = new MyConnectResponse();
        };
        static MyResponse response;
        static Exception exception;

        Because of = () =>
        {
            // Accept has a while loop that would block forever so start it in a new Thread/Context
            var accept_response = Task.Run(() => dispatcher.Accept(connect_response, cts.Token));
            var call_response = dispatcher.Call(request, cts.Token);

            var cancel_task = call_response
                .ContinueWith(t =>
                {
                    cts.Cancel();
                    cts.Dispose();
                });
            Task.WaitAll(accept_response, call_response, cancel_task);
            response = call_response.Result;
            exception = Catch.Exception(() => dispatcher.Call(request, CancellationToken.None).GetAwaiter().GetResult());
        };

        It should_write_one_connect_response_first = () => server_stream
            .Verify(_ =>
                _.WriteAsync(Moq.It.Is<MyServerMessage>(msg =>
                    connect_response_message.Equals(msg))),
                Moq.Times.Once);
        It should_then_write_one_request = () => server_stream
            .Verify(_ =>
                _.WriteAsync(Moq.It.Is<MyServerMessage>(msg =>
                    connect_request_message.Equals(msg))),
                Moq.Times.Once);
        It should_have_the_correct_callid_in_the_response = () =>
            response.Context.CallId.ShouldEqual(client_message.Response.Context.CallId);
        It should_have_the_correct_request_set = () =>
            connect_request_message.Request.ShouldEqual(request);
        It should_fail_on_second_call = () => exception.ShouldBeOfExactType<CannotPerformCallOnCompletedReverseCallConnection>();
    }
}
