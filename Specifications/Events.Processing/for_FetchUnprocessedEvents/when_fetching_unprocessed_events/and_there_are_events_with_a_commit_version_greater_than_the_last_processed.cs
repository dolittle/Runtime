// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using specs = Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Specs.when_fetching_unprocessed_events
{
    [Subject(typeof(IFetchUnprocessedEvents), nameof(IFetchUnprocessedEvents.GetUnprocessedEvents))]
    public class and_there_are_events_with_a_commit_version_greater_than_the_last_processed : given.a_fetch_unprocessed_events
    {
        static Artifact artifact;
        static SingleEventTypeEventStream event_stream;

        Establish context = () => artifact = specs.Artifacts.artifact_for_simple_event;

        Because of = () => event_stream = fetcher.GetUnprocessedEvents(artifact.Id, CommittedEventVersion.None);

        It should_return_a_stream_with_events = () => event_stream.IsEmpty.ShouldBeFalse();
        It should_contain_6_events = () => event_stream.Count().ShouldEqual(6);
    }
}