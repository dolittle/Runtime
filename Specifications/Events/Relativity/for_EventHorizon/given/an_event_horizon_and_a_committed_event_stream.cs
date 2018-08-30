using System;
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using Dolittle.Execution;
using Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon.given
{
    public class an_event_horizon_and_a_committed_event_stream : all_dependencies
    {
        protected static EventHorizon event_horizon;
        protected static Dolittle.Runtime.Events.Store.CommittedEventStream committed_event_stream;
        Establish context = () =>
        {
            var eventSource = new VersionedEventSource(Guid.NewGuid(), Guid.NewGuid());
            var correlationId = CorrelationId.New();

            committed_event_stream = new Store.CommittedEventStream(
                new CommitSequenceNumber(1),
                eventSource,
                CommitId.New(),
                CorrelationId.New(),
                DateTimeOffset.UtcNow,
                new EventStream(new []
                {
                    new EventEnvelope(
                        new EventId(),
                        new EventMetadata(
                            eventSource,
                            correlationId,
                            new Artifact(ArtifactId.New(), ArtifactGeneration.First),
                            new CausedBy { Value = "Someone" },
                            DateTimeOffset.UtcNow
                        ), new PropertyBag(new Dictionary<string, object>())
                    )
                })
            );

            event_horizon = new EventHorizon(logger.Object);
        };
    }
}