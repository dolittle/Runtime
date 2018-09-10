namespace Dolittle.Runtime.Events.for_EventSourceVersion.when_incrementing_the_commit
{
    using Machine.Specifications;

    [Subject(typeof(EventSourceVersion),"NextCommit")]
    public class on_a_version
    {
        static EventSourceVersion current;
        static EventSourceVersion result;

        Establish context = () => current = new EventSourceVersion(3,0);

        Because of = () => result = current.NextCommit();

        It should_be_the_next_commit = () => result.Commit.ShouldEqual(current.Commit + 1);
    }
}