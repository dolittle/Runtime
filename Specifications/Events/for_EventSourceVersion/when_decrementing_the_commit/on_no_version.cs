namespace Dolittle.Runtime.Events.for_EventSourceVersion.when_decrementing_the_commit
{
    using System;
    using Machine.Specifications;

    [Subject(typeof(EventSourceVersion),"PreviousCommit")]
    public class on_no_version
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => EventSourceVersion.NoVersion.PreviousCommit());

        It should_fail = () => exception.ShouldNotBeNull();
        It should_indicate_an_invalid_version = () => exception.ShouldBeOfExactType<InvalidEventSourceVersion>();
    }
}