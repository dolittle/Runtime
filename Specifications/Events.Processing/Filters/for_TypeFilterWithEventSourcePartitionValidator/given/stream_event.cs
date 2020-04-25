// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartitionValidator.given
{
    public static class stream_event
    {
        public static StreamEvent single() =>
            new StreamEvent(
                new CommittedEvent(
                    0,
                    DateTimeOffset.Now,
                    Guid.NewGuid(),
                    execution_contexts.create(),
                    artifacts.single(),
                    false,
                    ""),
                StreamPosition.Start,
                Guid.NewGuid(),
                Guid.NewGuid());
    }
}