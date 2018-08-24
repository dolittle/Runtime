using System;
using Dolittle.Artifacts;
using Dolittle.Events;
using Dolittle.Execution;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Events.Specs.for_EventSource.given
{
    public class a_committed_event_builder
    {
        protected static VersionedEventSource a_versioned_event_source;

        Establish context = () => 
        {
            a_versioned_event_source = new VersionedEventSource(EventSourceId.New(),ArtifactId.New());
        };

        public static CommittedEvent build_committed_event(VersionedEventSource versionedEventSource, IEvent @event, CommittedEventVersion version)
        {
            var metadata = new EventMetadata(versionedEventSource,CorrelationId.New(),new Artifact(ArtifactId.New(),1),"test",DateTime.UtcNow);
            return new CommittedEvent(version,metadata,EventId.New(),@event);
        }
    }
}