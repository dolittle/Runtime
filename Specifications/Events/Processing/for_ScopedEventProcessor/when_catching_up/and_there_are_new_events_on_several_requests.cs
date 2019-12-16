// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using specs = Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessor.when_catching_up
{
    [Subject(typeof(ScopedEventProcessor), "CatchUp")]
    public class and_there_are_new_events_on_several_requests : scoped_event_processors
    {
        static CommittedEventVersion version = new CommittedEventVersion(1, 1, 1);
        static CommittedEventVersion version_after_first_batch;
        static CommittedEventVersion final_version;

        Establish context = () =>
        {
            var event_artifact = specs.Artifacts.artifact_for_simple_event;
            var first_commit = specs.Events.Build(version);
            var second_commit = specs.Events.Build(first_commit.LastEventVersion);
            var events = first_commit.Events.Where(e => e.Metadata.Artifact == event_artifact).Select(e => e.ToCommittedEventEnvelope(first_commit.Sequence)).ToList();
            events.AddRange(second_commit.Events.Where(e => e.Metadata.Artifact == event_artifact).Select(e => e.ToCommittedEventEnvelope(second_commit.Sequence)));
            version_after_first_batch = events.Last().Version;

            var last_commit = specs.Events.Build(second_commit.LastEventVersion);
            var last_events = last_commit.Events.Where(e => e.Metadata.Artifact == event_artifact).Select(e => e.ToCommittedEventEnvelope(last_commit.Sequence)).ToList();

            final_version = last_events.Last().Version;
            offset_repository_simple_tenant.Setup(r => r.Get(simple_scoped_processor.ProcessorId)).Returns(version);
            unprocessed_events_fetcher_for_tenant_simple.Setup(f => f.GetUnprocessedEvents(event_artifact.Id, version)).Returns(new SingleEventTypeEventStream(events));
            unprocessed_events_fetcher_for_tenant_simple.Setup(f => f.GetUnprocessedEvents(event_artifact.Id, version_after_first_batch)).Returns(new SingleEventTypeEventStream(last_events));
            unprocessed_events_fetcher_for_tenant_simple.Setup(f => f.GetUnprocessedEvents(event_artifact.Id, final_version)).Returns(new SingleEventTypeEventStream(null));
        };

        Because of = () => simple_scoped_processor.CatchUp();

        It should_retrieve_the_offset_from_the_repository = () => offset_repository_simple_tenant.Verify(r => r.Get(simple_scoped_processor.ProcessorId), Times.Once());
        It should_retrieve_events_since_the_retrieved_last_processed_version = () => unprocessed_events_fetcher_for_tenant_simple.Verify(f => f.GetUnprocessedEvents(specs.Artifacts.artifact_for_simple_event.Id, version), Times.Once());
        It should_retrieve_events_since_the_events_it_has_processed = () => unprocessed_events_fetcher_for_tenant_simple.Verify(f => f.GetUnprocessedEvents(specs.Artifacts.artifact_for_simple_event.Id, version_after_first_batch), Times.Once());
        It should_retrieve_events_since_the_events_it_has_processed_second_time = () => unprocessed_events_fetcher_for_tenant_simple.Verify(f => f.GetUnprocessedEvents(specs.Artifacts.artifact_for_simple_event.Id, final_version), Times.Once());
        It should_have_the_version_set_from_the_last_event_it_processed = () => simple_scoped_processor.LastVersionProcessed.ShouldEqual(final_version);
    }
}