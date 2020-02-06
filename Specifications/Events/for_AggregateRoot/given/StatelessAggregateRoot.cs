// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot.given
{
    public class StatelessAggregateRoot : AggregateRoot
    {
        public StatelessAggregateRoot(EventSourceId eventSource)
            : base(eventSource)
        {
        }
    }
}