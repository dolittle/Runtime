// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.StreamProcessorStates.StreamProcessorId.non_default_scope;

class nonpartitioned : given.a_clean_event_store
{
    static readonly ScopeId scope_id = Guid.Parse("abef5f64-9916-4762-a234-527990bc6c7c");
    static readonly Guid event_processor_id = Guid.Parse("07f1ee20-ec03-41a1-8bf5-c66cf6359cdf");
    static readonly Guid source_stream_id = Guid.Parse("c9294a5d-2e85-4ae2-8411-878d5d4fb4ac");
    static readonly DateTimeOffset last_successfully_processed = DateTimeOffset.Now;

    static readonly Dolittle.Runtime.Events.Processing.Streams.StreamProcessorId stream_processor_id = new(scope_id, event_processor_id,
        source_stream_id);


    static readonly StreamProcessorState stream_processor_state =
        new(new StreamPosition(10), "", last_successfully_processed, 0, last_successfully_processed, false);


    Establish context = () =>
    {
        stream_processor_states.Persist(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult();
        retrieved_stream_processor_state = stream_processor_states.TryGetFor(stream_processor_id, CancellationToken.None).GetAwaiter().GetResult();
    };

    It should_have_retrieved_the_state_successfully = () => retrieved_stream_processor_state.Success.Should().BeTrue();

    It should_have_the_correct_stream_processor_state_type = () =>
        retrieved_stream_processor_state.Result.Should().BeOfType<StreamProcessorState>();

    It should_have_the_correct_stream_processor_state = () => retrieved_stream_processor_state.Result.Should().BeEquivalentTo(stream_processor_state);
    static Try<IStreamProcessorState> retrieved_stream_processor_state;
}