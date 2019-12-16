// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Events.Specs.for_EventSource.given
{
    public class an_event_source_with_2_uncommitted_events : a_stateful_event_source
    {
        private Establish context = () =>
                {
                    var firstEvent = new SimpleEvent();
                    event_source.Apply(firstEvent);
                    var secondEvent = new SimpleEvent();
                    event_source.Apply(secondEvent);
                };
    }
}