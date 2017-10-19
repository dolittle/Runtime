using System;
using doLittle.Runtime.Applications;
using doLittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Events.Specs.for_EventEnvelope.given
{
    public class an_event_envelope
    {
        protected static IEventEnvelope event_envelope;
        protected static Mock<IApplicationResourceIdentifier> event_identifier;
        protected static Mock<IApplicationResourceIdentifier> event_source_identifier;
        protected static EventSourceId event_source_id;
        protected static EventSourceVersion version;

        Establish context = () =>
        {
            event_source_id = EventSourceId.New();
            event_identifier = new Mock<IApplicationResourceIdentifier>();
            event_source_identifier = new Mock<IApplicationResourceIdentifier>();
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
