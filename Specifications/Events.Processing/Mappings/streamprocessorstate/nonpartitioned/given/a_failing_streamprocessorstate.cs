// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;


namespace Dolittle.Runtime.Events.Processing.Mappings.given;

public class a_failing_streamprocessorstate
{
    protected static readonly DateTimeOffset last_successfully_processed = DateTimeOffset.Now - TimeSpan.FromSeconds(10);
    protected static readonly DateTimeOffset retry_time = last_successfully_processed + TimeSpan.FromSeconds(30);

    protected static readonly StreamProcessorState stream_processor_state =
        new(new StreamPosition(10), "testing", retry_time, 2, last_successfully_processed, true);
}