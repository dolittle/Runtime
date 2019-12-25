// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Events.Specs.for_EventSource.given
{
    public class a_stateless_event_source : a_committed_event_builder
    {
        protected static StatelessEventSource event_source;
        protected static Guid event_source_id;

        Establish context = () =>
                {
                    event_source_id = Guid.NewGuid();
                    event_source = new StatelessEventSource(event_source_id);
                };
    }
}