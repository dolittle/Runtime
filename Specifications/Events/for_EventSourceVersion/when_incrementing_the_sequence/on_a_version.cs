namespace Dolittle.Runtime.Events.for_EventSourceVersion.when_incrementing_the_sequence
{
    using Machine.Specifications;

    [Subject(typeof(EventSourceVersion),"NextSequence")]
    public class on_a_version
    {
        static EventSourceVersion current;
        static EventSourceVersion result;

        Establish context = () => current = new EventSourceVersion(3,0);

        Because of = () => result = current.NextSequence();

        It should_be_the_same_commit = () => result.Commit.ShouldEqual(current.Commit);
        It should_be_the_next_sequence = () => result.Sequence.ShouldEqual(current.Sequence + 1);
    }
}