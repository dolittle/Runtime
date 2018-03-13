using System;
using Dolittle.Applications;
using Dolittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Specs.for_EventEnvelope.given
{
    public class an_event_envelope
    {
        protected static IEventEnvelope event_envelope;
        protected static Mock<IApplicationArtifactIdentifier> event_identifier;
        protected static Mock<IApplicationArtifactIdentifier> event_source_identifier;
        protected static EventSourceId event_source_id;
        protected static EventSourceVersion version;

        Establish context = () =>
        {
            event_source_id = EventSourceId.New();
            event_identifier = new Mock<IApplicationArtifactIdentifier>();
            event_source_identifier = new Mock<IApplicationArtifactIdentifier>();
            version = EventSourceVersion.Zero;
            event_envelope = new EventEnvelope(
                TransactionCorrelationId.NotSet,
                EventId.New(),
                EventSequenceNumber.Zero,
                EventSequenceNumber.Zero,
                EventGeneration.First,
                event_identifier.Object,
                event_source_id,
                event_source_identifier.Object,
                version,
                CausedBy.Unknown,
                DateTimeOffset.UtcNow
            );
        };

    }
}
