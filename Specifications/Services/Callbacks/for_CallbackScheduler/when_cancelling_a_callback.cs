// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Callbacks.for_CallbackScheduler
{
    public class when_cancelling_a_callback : given.all_dependencies
    {
        static Mock<Action> callback;
        static TimeSpan interval;
        static Mock<Action> callback_to_be_cancelled;

        Establish context = () =>
        {
            callback = new();
            callback_to_be_cancelled = new();
            interval = TimeSpan.FromMilliseconds(1000);
        };

        static int callCount;

        Because of = () =>
        {
            callCount = 3;
            using var result = scheduler.ScheduleCallback(callback.Object, interval);
            using var result_to_cancel = scheduler.ScheduleCallback(callback_to_be_cancelled.Object, interval);
            Task.Delay(interval).Wait();
            result_to_cancel.Dispose();
            Task.Delay(interval * callCount).Wait();
            host_application_cts.Cancel();
        };

        It should_have_called_the_first_callback_at_least_thrice = () => callback.Verify(_ => _(), Times.AtLeast(callCount));
        It should_have_called_the_cancelled_callback_only_once = () => callback_to_be_cancelled.Verify(_ => _(), Times.AtMostOnce());
    }
}
