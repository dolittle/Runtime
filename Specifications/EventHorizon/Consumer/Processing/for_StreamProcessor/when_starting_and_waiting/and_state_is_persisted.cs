// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using System.Threading.Tasks;
using System.Threading;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing.for_StreamProcessor.when_starting_and_waiting;

public class and_cancellation_has_been_requested : given.all_dependencies
{
    static CancellationTokenSource cts;
    Establish context = () =>
    {
        cts = new CancellationTokenSource(0);
    };
    static Task result;
    Because of = () =>
    {
        result = stream_processor.StartAndWait(cts.Token);
        Task.Delay(50).GetAwaiter().GetResult();
    };
    It should_complete = () => result.IsCompleted.ShouldBeTrue();
}