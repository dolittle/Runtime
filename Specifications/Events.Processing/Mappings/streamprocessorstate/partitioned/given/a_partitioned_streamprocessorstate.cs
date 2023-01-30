// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;


namespace Dolittle.Runtime.Events.Processing.Mappings.given;

public class a_partitioned_streamprocessorstate
{
    protected static readonly ScopeId scope_id = Guid.Parse("abef5f64-9916-4762-a234-527990bc6c7c");
    protected static readonly Guid event_processor_id = Guid.Parse("07f1ee20-ec03-41a1-8bf5-c66cf6359cdf");
    protected static readonly Guid source_stream_id = Guid.Parse("c9294a5d-2e85-4ae2-8411-878d5d4fb4ac");
    protected static readonly DateTimeOffset last_failed = DateTimeOffset.Now - TimeSpan.FromSeconds(10);
    protected static readonly StreamProcessorState stream_processor_state =
        new(new StreamPosition(10), ImmutableDictionary<PartitionId, FailingPartitionState>.Empty, DateTimeOffset.Now);
}