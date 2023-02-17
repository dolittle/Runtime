// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac.Extensions.DependencyInjection;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Services.Actors;
using Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Proto;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.actor.given;

public class a_dispatcher : for_ReverseCallDispatcher.given.a_dispatcher
{

    static ActorSystem actor_system;

    private Establish context = () =>
    {
        actor_system = new ActorSystem();
        var host = Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureServices(_ => _
                .AddSingleton(execution_context_creator.Object)
                .AddSingleton(Mock.Of<IMetricsCollector>()))
            .Build();
        
        var propsCreator = new CreateProps(host.Services);
        dispatcher = new ReverseCallDispatcherActor<MyClientMessage, MyServerMessage, MyConnectArguments, MyConnectResponse, MyRequest, MyResponse>.Wrapper(
            actor_system,
            propsCreator,
            RequestId.Generate(),
            pinged_connection.Object,
            new MyProtocol());
    };

    private Cleanup cleanup = () =>
    {
        dispatcher.Dispose();
        actor_system.DisposeAsync().AsTask().GetAwaiter().GetResult();
    };
}