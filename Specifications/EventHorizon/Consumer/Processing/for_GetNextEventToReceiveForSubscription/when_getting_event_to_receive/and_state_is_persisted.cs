// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using System;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing.for_GetNextEventToReceiveForSubscription.when_getting_event_to_receive;

public class and_state_is_persisted : given.all_dependencies
{
    static IStreamProcessorState subscription_state;
    Establish context = () =>
    {
        subscription_state = new StreamProcessorState(4, 10, DateTimeOffset.UtcNow);
        stream_processor_states
            .Setup(_ => _.TryGetFor(subscription_id, cancellation_token))
            .Returns(Task.FromResult(Try<IStreamProcessorState>.Succeeded(subscription_state)));
    };
    static StreamPosition result;
    Because of = () => result = get_next_event.GetNextEventToReceiveFor(subscription_id, cancellation_token).GetAwaiter().GetResult();

    It should_return_the_subscription_state_position = () => result.ShouldEqual(subscription_state.Position.StreamPosition);
}