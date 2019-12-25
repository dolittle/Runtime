// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.for_EventSourceVersion.when_decrementing_the_commit
{
    [Subject(typeof(EventSourceVersion), "PreviousCommit")]
    public class on_a_version
    {
        static EventSourceVersion current;
        static EventSourceVersion result;

        Establish context = () => current = new EventSourceVersion(3, 0);

        Because of = () => result = current.PreviousCommit();

        It should_be_the_previous_commit = () => result.Commit.ShouldEqual(current.Commit - 1);
    }
}