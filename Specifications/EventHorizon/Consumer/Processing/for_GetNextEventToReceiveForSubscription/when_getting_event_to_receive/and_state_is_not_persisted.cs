// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing.for_GetNextEventToReceiveForSubscription.when_getting_event_to_receive
{
    public class and_state_is_not_persisted : given.all_dependencies
    {
        Establish context = () =>
        {
            stream_processor_states
                .Setup(_ => _.TryGetFor(subscription_id, cancellation_token))
                .Returns(Task.FromResult(new Try<IStreamProcessorState>(false, null)));
        };
        static StreamPosition result;
        Because of = () => result = get_next_event.GetNextEventToReceiveFor(subscription_id, cancellation_token).GetAwaiter().GetResult();

        It should_return_the_start_position = () => result.ShouldEqual(StreamPosition.Start);
    }
}