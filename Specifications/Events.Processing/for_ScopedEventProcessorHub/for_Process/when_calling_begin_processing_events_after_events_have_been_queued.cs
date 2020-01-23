// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.for_Process
{
    [Subject(typeof(ScopedEventProcessingHub), nameof(IScopedEventProcessingHub.Process))]
    public class when_calling_begin_processing_events_after_events_have_been_queued : a_test_scoped_event_processing_hub
    {
        Establish context = () => commits.ForEach(c => hub.Process(c));

        Because of = () => hub.BeginProcessingEvents();

        It should_have_queued_all_the_event_streams = () => hub.Queued.Select(c => c.EventStream).ShouldContainOnly(commits);
        It should_process_all_the_event_streams = () => hub.Processed.Select(c => c.EventStream).ShouldContainOnly(commits);
    }
}