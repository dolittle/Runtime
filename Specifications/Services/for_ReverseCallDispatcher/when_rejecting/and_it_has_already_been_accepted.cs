// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_rejecting;

public class and_it_has_already_been_accepted : given.a_dispatcher
{
    static Exception exception;

    Establish context = () => dispatcher.Accept(new MyConnectResponse(), CancellationToken.None).GetAwaiter().GetResult();

    Because of = () => exception = Catch.Exception(() => dispatcher.Reject(new MyConnectResponse(), CancellationToken.None).GetAwaiter().GetResult());

    It should_fail_because_accept_has_already_been_called = () => exception.Should().BeOfType<ReverseCallDispatcherAlreadyAccepted>();
}