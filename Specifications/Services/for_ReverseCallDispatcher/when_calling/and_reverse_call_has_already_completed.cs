// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_calling
{
    public class and_reverse_call_has_already_completed : given.a_dispatcher
    {
        Establish context = () => dispatcher.Accept(new(), new CancellationToken(true)).GetAwaiter().GetResult();

        static Exception exception;
        Because of = () => exception = Catch.Exception(() => dispatcher.Call(new(), CancellationToken.None).GetAwaiter().GetResult());

        It should_fail = () => exception.ShouldBeOfExactType<CannotPerformCallOnCompletedReverseCallConnection>();
    }
}
