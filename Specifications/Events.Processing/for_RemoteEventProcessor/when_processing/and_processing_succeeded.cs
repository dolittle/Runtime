// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteEventProcessor.when_processing
{
    public class and_processing_succeeded : given.all_dependencies
    {
        static RemoteEventProcessor remote_processor;
        static IProcessingResult result;

        Establish context = () => remote_processor = new RemoteEventProcessor(event_processor_id, Processing.given.a_remote_processor_service(succeeded_handling_result), Moq.Mock.Of<ILogger>());

        Because of = async () => result = await remote_processor.Process(an_event, partition_id).ConfigureAwait(false);

        It should_succeed_processing = () => result.Succeeded.ShouldEqual(true);
        It should_not_retry_processing = () => result.Retry.ShouldEqual(false);
    }
}