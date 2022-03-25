// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Machine.Specifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Dolittle.Runtime.Services.Callbacks.for_CallbackScheduler.given;

public class all_dependencies
{
    protected static ICallbackScheduler scheduler;
    protected static Mock<IHostApplicationLifetime> host_application_lifetime;
    protected static CancellationTokenSource host_application_cts;
    Establish context = () =>
    {
        host_application_cts = new CancellationTokenSource();
        host_application_lifetime = new Mock<IHostApplicationLifetime>();
        host_application_lifetime.Setup(_ => _.ApplicationStopping).Returns(host_application_cts.Token);
        scheduler = new CallbackScheduler(
            host_application_lifetime.Object,
            NullLoggerFactory.Instance, 
            Mock.Of<IMetricsCollector>());
    };
}