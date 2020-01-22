// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.for_Process
{
    [Subject(typeof(ScopedEventProcessingHub), nameof(IScopedEventProcessingHub.Process))]
    public class when_processing_a_stream_without_begin_processing_events_being_called : a_test_scoped_event_processing_hub
    {
        Because of = () => commits.ForEach(c => hub.Process(c));

        It should_not_process_any_events = () => hub.Processed.Any().ShouldBeFalse();
        It should_queue_all_the_event_streams = () => hub.Queued.Select(c => c.EventStream).ShouldContainOnly(commits);
    }
}