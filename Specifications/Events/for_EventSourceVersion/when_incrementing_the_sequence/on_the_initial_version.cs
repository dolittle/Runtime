namespace Dolittle.Runtime.Events.for_EventSourceVersion.when_incrementing_the_sequence
{
    using Machine.Specifications;

    [Subject(typeof(EventSourceVersion),"NextSequence")]
    public class on_the_initial_version
    {
        static EventSourceVersion result;

        Because of = () => result = EventSourceVersion.Initial.NextSequence();

        It should_be_the_same_commit = () => result.Commit.ShouldEqual(EventSourceVersion.Initial.Commit);
        It should_be_the_next_sequence = () => result.Sequence.ShouldEqual(EventSourceVersion.Initial.Sequence + 1);
    }
}