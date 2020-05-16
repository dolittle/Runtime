// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Security;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Producer.for_EventExtensions.when_converting_to_committed_event_horizon_event
{
    public class and_there_are_claims
    {
        static CommittedEvent committed_event;
        static Events.Contracts.CommittedEvent result;

        Establish context = () => committed_event = new CommittedEvent(
            0,
            DateTimeOffset.Now,
            Guid.NewGuid(),
            execution_contexts.create_with_claims(new Claims(new[] { new Claim("name", "value", "valueType") })),
            artifacts.create(),
            false,
            "content");

        Because of = () => result = committed_event.ToCommittedEventHorizonEvent();

        It should_have_
        It should_not_have_any_claims = () => result.ExecutionContext.ToExecutionContext().Claims.ShouldEqual(Claims.Empty);
    }
}