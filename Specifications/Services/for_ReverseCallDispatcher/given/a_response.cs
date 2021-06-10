// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.given
{
    public class a_response : a_dispatcher
    {
        protected static MyClientMessage client_message;
        protected static MyServerMessage connect_response_message;
        protected static MyServerMessage connect_request_message;
        protected static CancellationTokenSource cts;
        protected static Execution.ExecutionContext execution_context;

        Establish context = () =>
        {
            client_stream
                .Setup(_ => _.MoveNext(Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            client_message = new()
            {
                Response = new()
                {
                    Context = new()
                }
            };

            // return a blank response to keep the while loop going, this gets changed later to break the loop
            client_stream
                .SetupGet(_ => _.Current)
                .Returns(client_message);

            server_stream
                .Setup(_ => _.WriteAsync(Moq.It.IsAny<MyServerMessage>()))
                .Callback<MyServerMessage>(server_message =>
                {
                    if (server_message.Request?.Context?.CallId != null)
                    {
                        connect_request_message = server_message;
                        // set the correct protobuf CallId from the request into the response
                        client_message.Response.Context.CallId = server_message.Request.Context.CallId;
                    }
                    else
                    {
                        connect_response_message = server_message;
                    }
                })
                .Returns(Task.FromResult(true));

            execution_context = execution_contexts.create();
            execution_context_manager
                .SetupGet(_ => _.Current)
                .Returns(execution_context);

            cts = new();
        };
    }
}
