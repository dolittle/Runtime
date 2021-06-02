// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.for_CallbackScheduler
{
    public class when_registering_a_callback
    {
        static Mock<Action> callback;
        static ICanScheduleCallbacks scheduler;
        static TimeSpan interval;

        Establish context = () =>
        {
            callback = new Mock<Action>();
            scheduler = new CallbackScheduler();
            interval = TimeSpan.FromMilliseconds(250);
        };

        static int callCount;

        Because of = () =>
        {
            callCount = 3;
            using (var result = scheduler.RegisterCallback(callback.Object, interval))
            {
                Task.Delay(interval * callCount).Wait();
            }
            // wait an extra interval so that we can be sure that the timer was disposed
            // instead of some race condition happening
            Task.Delay(interval).Wait();
        };

        It should_have_been_called_thrice = () => callback.Verify(_ => _(), Times.Exactly(callCount + 1));
    }
}
