// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_converting_to_event_horizon_committed_event.given;
using Dolittle.Runtime.Events;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_converting_to_event_horizon_committed_event;

public class with_non_guid_event_source
{
    static EventSourceId event_source;
    static Contracts.EventHorizonCommittedEvent result;
    
    Establish context = () => event_source = "an event source";

    Because of = () => result = a_committed_event.with_event_source(event_source).ToEventHorizonCommittedEvent();

    It should_not_set_legacy_event_source_field = () => result.EventSourceIdLegacy.Should().BeNull();
    It should_set_event_source_field = () => result.EventSourceId.Should().Be(event_source.Value);
}