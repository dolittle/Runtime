// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteEventProcessor
{
    public class when_creating_remote_event_processor : given.all_dependencies
    {
        static EventProcessor remote_processor;

        Because of = () => remote_processor = new EventProcessor(event_processor_id, remote_processor_service_mock.Object, Moq.Mock.Of<ILogger>());

        It should_have_the_correct_identifier = () => remote_processor.Identifier.ShouldEqual(event_processor_id);
    }
}