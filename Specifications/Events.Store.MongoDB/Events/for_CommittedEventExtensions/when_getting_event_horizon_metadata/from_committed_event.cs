// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_CommittedEventExtensions.when_getting_event_horizon_metadata;

public class from_committed_event
{
    static CommittedEvent committed_event;
    static EventHorizonMetadata result;

    Establish context = () =>
    {
        committed_event = committed_events.a_committed_event(random.event_log_sequence_number);
    };

    Because of = () => result = committed_event.GetEventHorizonMetadata();

    private It should_have_empty_consent = () => result.ShouldBeNull();
}