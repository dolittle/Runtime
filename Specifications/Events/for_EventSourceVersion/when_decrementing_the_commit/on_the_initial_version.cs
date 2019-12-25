// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.for_EventSourceVersion.when_decrementing_the_commit
{
    [Subject(typeof(EventSourceVersion), "PreviousCommit")]
    public class on_the_initial_version
    {
        static EventSourceVersion result;

        Because of = () => result = EventSourceVersion.Initial.PreviousCommit();

        It should_be_no_version = () => result.ShouldEqual(EventSourceVersion.NoVersion);
    }
}