// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.Callbacks.for_CallbackScheduler
{
    public class when_registering_a_callback : given.all_dependencies
    {
        static Mock<Action> callback;
        static TimeSpan interval;

        Establish context = () =>
        {
            callback = new Mock<Action>();
            interval = TimeSpan.FromMilliseconds(1000);
        };

        static int callCount;

        Because of = () =>
        {
            callCount = 3;
            using (var result = scheduler.ScheduleCallback(callback.Object, interval))
            {
                Task.Delay(interval * (callCount + 1)).Wait();
            }
            host_application_cts.Cancel();
        };

        It should_have_been_called_at_least_thrice = () => callback.Verify(_ => _(), Times.AtLeast(callCount));
    }
}
