// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_converting_to_event_horizon_committed_event.given;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_converting_to_event_horizon_committed_event;

public class with_guid_event_source
{
    static EventSourceId event_source;
    static Contracts.EventHorizonCommittedEvent result;
    
    Establish context = () => event_source = "b4cbb675-e802-4975-9c20-895323199dec";

    Because of = () => result = a_committed_event.with_event_source(event_source).ToEventHorizonCommittedEvent();

    It should_set_legacy_event_source_field = () => result.EventSourceIdLegacy.ShouldEqual(new Guid(event_source).ToProtobuf());
    It should_set_event_source_field = () => result.EventSourceId.ShouldEqual(event_source.Value);
}