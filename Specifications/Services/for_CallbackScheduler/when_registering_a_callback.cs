// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Services.for_CallbackScheduler
{
    public class when_registering_a_callback
    {
        static Mock<Action> callback;
        static ICallbackScheduler scheduler;
        static TimeSpan interval;
        static Mock<IHostApplicationLifetime> host_applictation_lifetime;
        static CancellationTokenSource cts;

        Establish context = () =>
        {
            callback = new();
            host_applictation_lifetime = new();
            cts = new();
            host_applictation_lifetime.Setup(_ => _.ApplicationStopping).Returns(cts.Token);
            scheduler = new CallbackScheduler(host_applictation_lifetime.Object, Mock.Of<ILogger>());
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
            cts.Cancel();
        };

        It should_have_been_called_at_least_thrice = () => callback.Verify(_ => _(), Times.AtLeast(callCount));
    }
}
