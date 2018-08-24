using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using Dolittle.Events;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Store;
using Dolittle.Execution;
using Dolittle.Artifacts;
using System;

namespace Dolittle.Events.Specs.for_EventSource
{
    [Subject(Subjects.reapplying_events)]
    public class when_reapplying_a_stream_of_committed_events : given.a_stateful_event_source
    {
        static Dolittle.Runtime.Events.CommittedEventStream event_stream;
        Establish context =
            () =>
            {
                var versioned_event_source = a_versioned_event_source_for(event_source_id);

                var first_event = new SimpleEvent();
                var first_committed_event = build_committed_event(versioned_event_source, first_event, new CommittedEventVersion(1,1,0));

                var second_event = new SimpleEvent();
                var second_committed_event = build_committed_event(versioned_event_source, second_event, new CommittedEventVersion(1,1,1));

                var third_event = new SimpleEvent();
                var third_committed_event = build_committed_event(versioned_event_source, third_event, new CommittedEventVersion(2,2,0));

                event_stream = new Dolittle.Runtime.Events.CommittedEventStream(event_source_id,new[] {
                    first_committed_event,
                    second_committed_event,
                    third_committed_event
                });
            };

        Because of = () => event_source.ReApply(event_stream);

        It should_not_add_the_events_to_the_uncommited_events = () => event_source.UncommittedEvents.ShouldBeEmpty();
        It should_increment_the_commit_of_the_version = () => event_source.Version.Commit.ShouldEqual(3UL);
        It should_being_with_a_sequence_of_zero = () => event_source.Version.Sequence.ShouldEqual(0u);
        It should_have_applied_the_event = () => event_source.EventApplied.ShouldBeTrue();
    }
}