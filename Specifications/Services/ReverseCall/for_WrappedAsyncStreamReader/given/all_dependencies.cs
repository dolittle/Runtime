// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamReader.given;

public class all_dependencies : ReverseCalls.given.all_dependencies
{
    protected static Mock<ReverseCallContextReceived> reverse_call_context_received;
    protected static Mock<ReverseCallContextNotReceivedInFirstMessage> reverse_call_context_not_received_in_first_message;

    Establish context = () =>
    {
        reverse_call_context_received = new Mock<ReverseCallContextReceived>();
        reverse_call_context_not_received_in_first_message = new Mock<ReverseCallContextNotReceivedInFirstMessage>();
    };
}