// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Processing;
using Microsoft.Extensions.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing.for_EventProcessor.when_processing
{
    public class an_event : given.all_dependencies
    {
        static EventProcessor processor;
        static IProcessingResult result;

        Establish context = () => processor = new EventProcessor(consent_id, subscription_id, event_horizon_events_writer.Object, event_processor_policy, metrics, logger);

        Because of = () => result = processor.Process(@event, partition, default).GetAwaiter().GetResult();

        It should_write_event = () => event_horizon_events_writer.Verify(_ => _.Write(@event, consent_id, subscription_id.ScopeId, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_return_succeeded_processing_result = () => result.Succeeded.ShouldBeTrue();
    }
}
