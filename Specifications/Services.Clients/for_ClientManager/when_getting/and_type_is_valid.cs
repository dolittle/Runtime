// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Clients.for_ClientManager.when_getting
{
    public class and_type_is_valid : given.a_client_manager
    {
        class MyClient : ClientBase
        {
            public MyClient(CallInvoker callInvoker)
                : base(callInvoker)
            {
                Invoker = callInvoker;
            }

            public CallInvoker Invoker { get; }
        }

        static MyClient result;
        static Mock<CallInvoker> call_invoker;

        Establish context = () =>
        {
            call_invoker = new Mock<CallInvoker>();
            call_invoker_manager.Setup(_ => _.GetFor(typeof(MyClient), Moq.It.IsAny<string>(), Moq.It.IsAny<int>())).Returns(call_invoker.Object);
        };

        Because of = () => result = client_manager.Get(typeof(MyClient)) as MyClient;

        It should_pass_the_call_invoker = () => result.Invoker.ShouldEqual(call_invoker.Object);
    }
}