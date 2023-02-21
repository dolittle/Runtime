// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Services.Configuration;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls.for_ReverseCallStreamWriterActor.given;

public class a_wrapped_stream_writer : all_dependencies
{
    protected static Mock<IAsyncStreamWriter<a_message>> original_writer;
    protected static IReverseCallStreamWriter<a_message> writer;
    static ActorSystem actor_system;

    private Establish context = () =>
    {
        actor_system = new ActorSystem();
        original_writer = new Mock<IAsyncStreamWriter<a_message>>();
        var host = Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureLogging(_ => _.ClearProviders())
            .ConfigureServices(_ => _
                .AddSingleton(Mock.Of<IMetricsCollector>()))
            .Build();
        var factory = new ReverseCallStreamWriterFactory(
            actor_system,
            new CreateProps(host.Services),
            new OptionsWrapper<ReverseCallsConfiguration>(new ReverseCallsConfiguration{UseActors = true}),
            metrics,
            NullLoggerFactory.Instance);
        writer = factory.CreateWriter(request_id, original_writer.Object, message_converter.Object, cancellation_token, null);
    };

    Cleanup cleanup = () =>
    {
        writer.Dispose();
        actor_system.DisposeAsync().AsTask().GetAwaiter().GetResult();
    };
}