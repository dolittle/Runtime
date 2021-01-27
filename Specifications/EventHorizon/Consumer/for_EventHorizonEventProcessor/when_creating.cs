// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_EventHorizonEventProcessor
{
    public class when_creating : given.all_dependencies
    {
        static EventProcessor processor;
        Because of = () => processor = new EventProcessor(consent_id, subscription_id, event_horizon_events_writer.Object, event_processor_policy, Moq.Mock.Of<ILogger>());

        It should_have_the_correct_identifier = () => processor.Identifier.Value.ShouldEqual(subscription_id.ProducerTenantId.Value);
    }
}