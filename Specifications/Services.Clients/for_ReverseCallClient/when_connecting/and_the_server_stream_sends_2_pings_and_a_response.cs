// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.Clients.for_ReverseCallClient.given.a_client;
using Dolittle.Services.Contracts;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ReverseCallClient.when_connecting
{
    public class and_the_server_stream_sends_2_pings_and_a_response : given.a_reverse_call_client
    {
        static bool result;
        static Execution.ExecutionContext execution_context;
        static MyConnectResponse connect_response;

        Establish context = () =>
        {
            execution_context = given.execution_contexts.create();
            execution_context_manager.SetupGet(_ => _.Current).Returns(execution_context);
            server_to_client_stream.Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            connect_response = new MyConnectResponse();
            var ping_message = new MyServerMessage
            {
                Ping = new Ping()
            };
            var connect_message = new MyServerMessage
            {
                ConnectResponse = connect_response
            };
            server_to_client_stream
                .SetupSequence(_ => _.Current)
                .Returns(ping_message)
                .Returns(ping_message)
                .Returns(() =>
                {
                    // setup all the subsequent calls to Current, otherwise the sequence returns null
                    server_to_client_stream.SetupGet(_ => _.Current).Returns(connect_message);
                    return connect_message;
                });
        };

        Because of = () => result = reverse_call_client.Connect(new MyConnectArguments(), CancellationToken.None).GetAwaiter().GetResult();

        It should_return_true = () => result.ShouldBeTrue();
        It should_respond_with_a_pong_twice = () => client_to_server_stream.Verify(_ => _.WriteAsync(Moq.It.Is<MyClientMessage>(_ => _.Pong != default)), Moq.Times.Exactly(2));
        It should_set_correct_connect_response = () => reverse_call_client.ConnectResponse.ShouldEqual(connect_response);
    }
}
