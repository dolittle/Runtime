// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Integration.Tests.Events.Processing.StreamProcessorStates.StreamProcessorId;

class with_no_events : given.a_clean_event_store
{
    static readonly ScopeId scope_id = Guid.Parse("abef5f64-9916-4762-a234-527990bc6c7c");
    static readonly Guid event_processor_id = Guid.Parse("07f1ee20-ec03-41a1-8bf5-c66cf6359cdf");
    static readonly Guid source_stream_id = Guid.Parse("c9294a5d-2e85-4ae2-8411-878d5d4fb4ac");

    static readonly Dolittle.Runtime.Events.Processing.Streams.StreamProcessorId stream_processor_id = new(scope_id, event_processor_id,
        source_stream_id);

    static Try<IStreamProcessorState> result;

    Establish context = () => { result = stream_processor_states.TryGetFor(stream_processor_id, CancellationToken.None).GetAwaiter().GetResult(); };

    It should_return_a_failure = () => result.Success.Should().BeFalse();
    
    It should_return_a_stream_processor_state_not_found_exception = () => result.Exception.Should().BeOfType<StreamProcessorStateDoesNotExist>();
}