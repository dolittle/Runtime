// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;


namespace Dolittle.Runtime.Events.Processing.Mappings.given;

public class a_streamprocessorstate
{
    protected static readonly StreamProcessorState stream_processor_state = new(new StreamPosition(10), DateTimeOffset.Now);
}