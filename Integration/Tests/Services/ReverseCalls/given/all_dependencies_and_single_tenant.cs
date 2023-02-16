// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Clients;
using Dolittle.Runtime.Services.Configuration;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Integration.Tests.Services.given;
using Machine.Specifications;

namespace Integration.Tests.Services.ReverseCalls.given;

class all_dependencies_and_single_tenant : Services.given.all_dependencies_and_single_tenant
{
    internal static Func<IAsyncStreamReader<ClientToRuntimeMessage>, IServerStreamWriter<RuntimeToClientMessage>, ServerCallContext, Task> on_service_connect;
    internal static TaskCompletionSource connect_completion_source;
    protected static Try<(IReverseCallDispatcher<ClientToRuntimeMessage, RuntimeToClientMessage, RegistrationRequest, RegistrationResponse, Request, Response>, RegistrationArguments)> connect_result;
    protected static IReverseCallClient<RegistrationRequest, RegistrationResponse, Request, Response> reverse_call_client;

    Establish context = () =>
    {
        connect_completion_source = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        reverse_call_client = null;
        on_service_connect = null;
        connect_result = null;
    };

    protected static void setup(Func<IAsyncStreamReader<ClientToRuntimeMessage>, IServerStreamWriter<RuntimeToClientMessage>, ServerCallContext, Task> on_service_connect, TimeSpan ping_interval)
    {
        all_dependencies_and_single_tenant.on_service_connect = on_service_connect;
        reverse_call_client = reverse_call_clients.GetFor(
            new basic_with_ping_pong_reverse_call_client_protocol(),
            "localhost",
            new EndpointsConfiguration().Private.Port,
            ping_interval);
        
    }

    Cleanup cleanup = () => reverse_call_client.Dispose();
}

[PrivateService]
public class service : BasicReverseCallService.BasicReverseCallServiceBase
{
    public override async Task Connect(IAsyncStreamReader<ClientToRuntimeMessage> requestStream, IServerStreamWriter<RuntimeToClientMessage> responseStream, ServerCallContext context)
    {
        try
        {
            var waitForCancel = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            await using var registration = context.CancellationToken.Register(() => waitForCancel.TrySetResult()).ConfigureAwait(false);
            await all_dependencies_and_single_tenant.on_service_connect(requestStream, responseStream, context).ConfigureAwait(false);
            await waitForCancel.Task.ConfigureAwait(false);

            all_dependencies_and_single_tenant.connect_completion_source.SetResult();
        }
        catch (Exception e)
        {
            all_dependencies_and_single_tenant.connect_completion_source.SetException(e);
            throw;
        }
    }
}
