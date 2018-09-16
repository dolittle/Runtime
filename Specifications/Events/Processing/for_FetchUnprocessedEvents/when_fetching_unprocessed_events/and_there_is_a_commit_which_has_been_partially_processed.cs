namespace Dolittle.Runtime.Events.Specs.when_fetching_unprocessed_events
{
    using Dolittle.Runtime.Events.Processing;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using Machine.Specifications;
    using Dolittle.Artifacts;
    using Dolittle.Runtime.Events.Store;
    using System.Linq;

    [Subject(typeof(IFetchUnprocessedEvents),nameof(IFetchUnprocessedEvents.GetUnprocessedEvents))]
    public class and_there_is_a_commit_which_has_been_partially_processed : given.a_fetch_unprocessed_events
    {
        static Artifact artifact;
        static CommittedEventVersion version;
        static SingleEventTypeEventStream event_stream;

        Establish context = () => 
        {
            artifact = specs.Artifacts.artifact_for_simple_event;
            version = new CommittedEventVersion(2,2,0);
        };

        Because of = () => event_stream = fetcher.GetUnprocessedEvents(artifact.Id,version);

        It should_return_a_stream_with_events = () => event_stream.IsEmpty.ShouldBeFalse();
        It should_contain_3_events = () => event_stream.Count().ShouldEqual(3); //1 event from the partial and 2 from the next commit
    }    
}