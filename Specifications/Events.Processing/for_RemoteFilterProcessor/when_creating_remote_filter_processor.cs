// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteFilterProcessor
{
    public class when_creating_remote_filter_processor : given.all_dependencies
    {
        static RemoteFilterProcessor remote_filter;

        Because of = () => remote_filter = new RemoteFilterProcessor(event_processor_id, target_stream_id, Processing.given.a_remote_filter_service(new FailedFilteringResult()), new in_memory_event_to_stream_writer());

        It should_have_the_correct_identifier = () => remote_filter.Identifier.ShouldEqual(event_processor_id);
    }
}