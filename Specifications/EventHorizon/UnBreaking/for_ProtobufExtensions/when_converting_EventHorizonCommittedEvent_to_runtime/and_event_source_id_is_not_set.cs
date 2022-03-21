// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_converting_EventHorizonCommittedEvent_to_runtime;

public class and_event_source_id_is_not_set : given.a_committed_event
{
    static EventSourceId event_source; 
    static CommittedEvent result;

    Establish context = () =>
    {
        event_source = "ee0f17db-797a-46c7-8eab-ef9725aa8c98";
        committed_event.EventSourceId = null;
        committed_event.EventSourceIdLegacy = new Guid(event_source).ToProtobuf();
        
    };

    Because of = () => result = committed_event.ToCommittedEvent();

    It should_have_the_correct_event_source = () => result.EventSource.ShouldEqual(event_source);
}