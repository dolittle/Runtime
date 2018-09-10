namespace Dolittle.Runtime.Events.for_EventSourceVersion.when_decrementing_the_commit
{
    using System;
    using Machine.Specifications;

    [Subject(typeof(EventSourceVersion),"PreviousCommit")]
    public class on_the_initial_version
    {
        static EventSourceVersion result;

        Because of = () => result = EventSourceVersion.Initial.PreviousCommit();

        It should_be_no_version = () => result.ShouldEqual(EventSourceVersion.NoVersion);
    }
}