// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Clients;
using Dolittle.Runtime.Services.Configuration;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Integration.Tests.Services.given;

class all_dependencies_and_single_tenant : a_runtime_with_a_single_tenant
{
    protected static IInitiateReverseCallServices reverse_call_initiator;
    protected static IReverseCallClients reverse_call_clients;
    protected static EndpointsConfiguration endpoints;
    
    Establish context = () =>
    {
        reverse_call_initiator = runtime.Host.Services.GetRequiredService<IInitiateReverseCallServices>();
        reverse_call_clients = runtime.Host.Services.GetRequiredService<IReverseCallClients>();
        endpoints = runtime.Host.Services.GetRequiredService<IOptions<EndpointsConfiguration>>().Value;
    };
}