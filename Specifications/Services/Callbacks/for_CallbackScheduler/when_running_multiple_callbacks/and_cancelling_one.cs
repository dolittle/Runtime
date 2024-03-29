// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Callbacks.for_CallbackScheduler.when_running_multiple_callbacks;

public class and_cancelling_one : given.all_dependencies
{
    static Mock<Action> not_cancelled_callback;
    static TimeSpan interval;
    static Mock<Action> callback_to_be_cancelled;
    static int callCount;
    static IDisposable callback_to_be_cancelled_disposable;
    static IDisposable not_cancelled_callback_disposable;

    Establish context = () =>
    {
        not_cancelled_callback = new Mock<Action>();
        
        callback_to_be_cancelled = new Mock<Action>();
        callback_to_be_cancelled.Setup(_ => _()).Callback(() => callback_to_be_cancelled_disposable?.Dispose());
        
        interval = TimeSpan.FromMilliseconds(100);
        callCount = 3;
        not_cancelled_callback_disposable = scheduler.ScheduleCallback(not_cancelled_callback.Object, interval);
    };

    Because of = () =>
    {
        callback_to_be_cancelled_disposable = scheduler.ScheduleCallback(callback_to_be_cancelled.Object, interval);
        Task.Delay(interval * (callCount + 1)).GetAwaiter().GetResult();
    };

    It should_have_called_the_first_callback_at_least_thrice = () => not_cancelled_callback.Verify(_ => _(), Times.AtLeast(callCount));
    It should_have_called_the_cancelled_callback_only_once = () => callback_to_be_cancelled.Verify(_ => _(), Times.AtMostOnce());

    Cleanup clean = () =>
    {
        not_cancelled_callback_disposable.Dispose();
        host_application_cts.Cancel();
    };
}