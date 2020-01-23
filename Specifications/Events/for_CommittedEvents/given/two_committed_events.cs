// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Execution;
using Machine.Specifications;
using given = Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Specs.for_CommittedEvents.given
{
    public abstract class two_committed_events : given::Events
    {
        public static CommittedEvent first_event;
        public static CommittedEvent second_event;

        Establish context = () =>
        {
            first_event = new CommittedEvent(
                0,
                DateTimeOffset.Now,
                CorrelationId.New(),
                Microservice.New(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                event_one);

            second_event = new CommittedEvent(
                1,
                DateTimeOffset.Now,
                CorrelationId.New(),
                Microservice.New(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                event_one);
        };
    }
}