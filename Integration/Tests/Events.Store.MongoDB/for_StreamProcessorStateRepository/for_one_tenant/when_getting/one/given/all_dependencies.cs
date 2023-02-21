// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Integration.Tests.Events.Store.MongoDB.for_StreamProcessorStateRepository.for_one_tenant.when_getting.one.given;

public class all_dependencies : for_one_tenant.given.a_stream_processor_id
{
    protected static Try<IStreamProcessorState> retrieved_state;
    protected static IStreamProcessorState stored_state;
    
    protected static void get_stream_processor_state()
    {
        retrieved_state = stream_processor_state_repository.TryGet(stream_processor_id, CancellationToken.None).GetAwaiter().GetResult();
    }
    
    
}