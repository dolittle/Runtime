// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.for_EventSourceVersion.when_decrementing_the_commit
{
    [Subject(typeof(EventSourceVersion), "PreviousCommit")]
    public class on_no_version
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => EventSourceVersion.NoVersion.PreviousCommit());

        It should_fail = () => exception.ShouldNotBeNull();
        It should_indicate_an_invalid_version = () => exception.ShouldBeOfExactType<InvalidEventSourceVersion>();
    }
}