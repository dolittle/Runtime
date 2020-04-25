// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_EventHorizonEventProcessor
{
    public class when_creating : given.all_dependencies
    {
        static EventProcessor processor;
        Because of = () => processor = new EventProcessor(subscription, event_horizon_events_writer.Object, Moq.Mock.Of<ILogger>());

        It should_have_the_correct_identifier = () => processor.Identifier.Value.ShouldEqual(subscription.ProducerTenant.Value);
    }
}