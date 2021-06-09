// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.when_calling
{
    public class and_it_fails : given.a_response
    {
        static Exception exception;

        Because of = () =>
        {
            // force a NullReferenceException with a null request
            exception = Catch.Exception(() => dispatcher.Call(null, CancellationToken.None).GetAwaiter().GetResult());
        };

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<NullReferenceException>();
    }
}
