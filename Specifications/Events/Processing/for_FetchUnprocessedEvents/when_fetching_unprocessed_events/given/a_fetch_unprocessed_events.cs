namespace Dolittle.Runtime.Events.Specs.when_fetching_unprocessed_events.given
{
    using Dolittle.Artifacts;
    using Dolittle.Runtime.Events.Processing;
    using given = Dolittle.Runtime.Events.Specs.Processing.given;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using Dolittle.Runtime.Events.Store;
    using Machine.Specifications;
    using Moq;
    using System.Linq;
    using System.Collections.Generic;

    public class a_fetch_unprocessed_events
    {
        protected static IFetchUnprocessedEvents fetcher;
        protected static Mock<IEventStore> event_store;

        Establish context = () => 
        {
            
            event_store = new Mock<IEventStore>();
            event_store.Setup(es => es.FetchAllEventsOfTypeAfter(Moq.It.IsAny<ArtifactId>(),Moq.It.IsAny<CommitSequenceNumber>()))
                            .Returns((ArtifactId id, CommitSequenceNumber commit) => new SingleEventTypeEventStream(get_events_from(id,commit)) );
            fetcher = new FetchUnprocessedEvents(() => event_store.Object);
        };

        static IEnumerable<CommittedEventEnvelope> get_events_from(ArtifactId artifactId, CommitSequenceNumber commit)
        {
            var committed_event_streams = given.committed_event_streams();  
            var events = committed_event_streams.SelectMany(c => c.Events.Select(e => e.ToCommittedEventEnvelope(c.Sequence)))
                                                    .Where(e => e.Metadata.Artifact.Id == artifactId)
                                                        .OrderBy(c => c.Version);
            return events.Where(e => e.Version.Major >= commit).ToList();
        }

    }
}