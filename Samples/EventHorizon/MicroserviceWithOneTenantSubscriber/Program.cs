// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Applications.Configuration;
using Dolittle.Booting;
using Dolittle.Execution;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Microservices;
using Dolittle.Tenancy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Samples.MicroserviceWithOneTenantSubscriber
{
    static class Program
    {
        static async Task Main()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureLogging(_ => _.AddConsole());
            hostBuilder.UseEnvironment("Development");
            var host = hostBuilder.Build();
            var loggerFactory = host.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            var result = Bootloader.Configure(_ =>
            {
                _.UseLoggerFactory(loggerFactory);
                _.Development();
            }).Start();
            var consumerClient = result.Container.Get<IConsumerClient>();
            var microservices = result.Container.Get<IMicroservices>();
            var executionContextManager = result.Container.Get<IExecutionContextManager>();
            var boundedContextConfiguration = result.Container.Get<BoundedContextConfiguration>();
            executionContextManager.CurrentFor(boundedContextConfiguration.Application, boundedContextConfiguration.BoundedContext, TenantId.Development);
            var eventHorizon = new EventHorizon.EventHorizon(
                executionContextManager.Current.Microservice,
                executionContextManager.Current.Tenant,
                Guid.Parse("4e146c23-3b32-4eae-90bd-068f37844dc0"),
                TenantId.Development);
            var microserviceAddress = microservices.GetAddressFor(eventHorizon.ProducerMicroservice);
            _ = consumerClient.SubscribeTo(eventHorizon, Guid.Parse("8a37fe70-654e-4e47-91e6-0c9103bd0519"), Guid.Parse("eb356fbc-59f1-486a-909f-1a20eb1e9ee5"), Guid.Empty, microserviceAddress);
            await host.RunAsync().ConfigureAwait(false);
        }
    }
}
