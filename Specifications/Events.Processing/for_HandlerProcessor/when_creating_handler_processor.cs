// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_HandlerProcessor
{
    public class when_creating_handler_processor : given.all_dependencies
    {
        static RemoteEventProcessor handler_processor;

        Because of = () => handler_processor = new RemoteEventProcessor(tenant_id, event_processor_id, handler_service_mock.Object);

        It should_have_the_correct_identifier = () => handler_processor.Identifier.ShouldEqual(event_processor_id);
    }
}