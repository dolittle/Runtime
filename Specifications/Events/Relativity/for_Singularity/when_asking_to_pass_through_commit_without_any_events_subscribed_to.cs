using System;
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.PropertyBags;
using Machine.Specifications;
using Dolittle.Execution;
using Dolittle.Runtime.Events;
using Dolittle.Collections;


namespace Dolittle.Runtime.Events.Relativity.for_Singularity
{
    public class when_asking_to_pass_through_commit_without_any_events_subscribed_to : given.a_singularity_with_a_tunnel_and_no_events_to_subscribe_to
    {
        static Store.CommittedEventStream stream;
        static bool result;

        Establish context = () => 
        {
            var versionedEventSource = new VersionedEventSource(Guid.NewGuid(), ArtifactId.New());
            var correlationId = CorrelationId.New();
            stream = new Store.CommittedEventStream(
                0L,
                versionedEventSource,
                Store.CommitId.New(),
                correlationId,
                DateTimeOffset.UtcNow,
                new Store.EventStream(new[] {
                    new EventEnvelope(
                        EventId.New(),
                        new EventMetadata(versionedEventSource,correlationId, Artifact.New(), "", DateTimeOffset.UtcNow),
                        new PropertyBag(new NullFreeDictionary<string, object>())
                    )
                })
            );
        };

        Because of = () => result = singularity.CanPassThrough(stream);

        It should_not_be_able_to_pass_through = () => result.ShouldBeFalse();
    }
}