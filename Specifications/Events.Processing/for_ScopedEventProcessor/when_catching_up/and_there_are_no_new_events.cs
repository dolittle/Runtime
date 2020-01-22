// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using specs = Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessor.when_catching_up
{
    [Subject(typeof(ScopedEventProcessor), "CatchUp")]
    public class and_there_are_no_new_events : scoped_event_processors
    {
        static CommittedEventVersion version = new CommittedEventVersion(1, 1, 1);

        Establish context = () => offset_repository_simple_tenant.Setup(r => r.Get(simple_scoped_processor.ProcessorId)).Returns(version);
        Because of = () => simple_scoped_processor.CatchUp();

        It should_retrieve_the_offset_from_the_repository = () => offset_repository_simple_tenant.Verify(r => r.Get(simple_scoped_processor.ProcessorId), Times.Once());
        It should_retrieve_events_since_the_last_processed_version = () => unprocessed_events_fetcher_for_tenant_simple.Verify(f => f.GetUnprocessedEvents(specs.Artifacts.artifact_for_simple_event.Id, version), Times.Once());
        It should_have_the_version_set_from_the_repository = () => simple_scoped_processor.LastVersionProcessed.ShouldEqual(version);
    }
}