using Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.when_getting_unique.stream_events.and_there_are_one_event_to_write_and_one_stored.having_the_same_event_log_sequence_number;

public class but_different_partition : given.all_dependencies
{
    Establish context = () =>
    {
        event_log_sequence_number = event_log_sequence_number with {Value = 3};
        stream_events_to_write.Add(a_stream_event.at_event_log_position(event_log_sequence_number).with_partition(partition with {Value = "some partition"}));
        stored_stream_events.Add(a_stream_event.at_event_log_position(event_log_sequence_number).with_partition(partition with {Value = "some other partition"}));
    };

    Because of = get_unique_stream_events;

    It should_not_be_successful = () => result.ShouldBeFalse();
    It should_have_the_correct_duplicate_event_log = () => duplicate_event_log_sequence_number.ShouldEqual(event_log_sequence_number);
}