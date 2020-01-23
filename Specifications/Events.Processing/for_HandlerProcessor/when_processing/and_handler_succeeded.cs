// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_HandlerProcessor.when_processing
{
    public class and_handler_succeeded : given.all_dependencies
    {
        static HandlerProcessor handler_processor;
        static IProcessingResult result;

        Establish context = () => handler_processor = new HandlerProcessor(tenant_id, event_processor_id, Processing.given.a_handler_service(succeeded_handling_result));

        Because of = async () => result = await handler_processor.Process(an_event);

        It should_return_processing_result = () => result.ShouldNotBeNull();
        It should_return_succeeded_processing = () => result.Value.ShouldEqual(ProcessingResultValue.Succeeded);
    }
}