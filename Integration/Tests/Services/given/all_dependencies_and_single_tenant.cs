// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Clients;
using Dolittle.Runtime.Services.Configuration;
using Dolittle.Services.Contracts;
using Google.Protobuf.WellKnownTypes;
using Integration.Shared;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Event = Dolittle.Runtime.Events.Store.MongoDB.Events.Event;

namespace Integration.Tests.Events.Processing.given;

class all_dependencies_and_single_tenant : a_runtime_with_a_single_tenant
{
    protected static IInitiateReverseCallServices reverse_call_initiator;
    protected static IReverseCallClients reverse_call_clients;
    protected static EndpointsConfiguration endpoints = new();
    
    Establish context = () =>
    {
        reverse_call_initiator = runtime.Host.Services.GetRequiredService<IInitiateReverseCallServices>();
        reverse_call_clients = runtime.Host.Services.GetRequiredService<IReverseCallClients>();
        reverse_call_clients.GetFor(
    };
}

class protocol : IReverseCallServiceProtocol<Empty,Empty,Empty,Empty,Empty, Empty, Empty>
{
    public Empty GetConnectArguments(Empty message)
        => throw new NotImplementedException();

    public void SetConnectResponse(Empty arguments, Empty message)
    {
        throw new NotImplementedException();
    }

    public void SetRequest(Empty request, Empty message)
    {
        throw new NotImplementedException();
    }

    public Empty GetResponse(Empty message)
        => throw new NotImplementedException();

    public ReverseCallArgumentsContext GetArgumentsContext(Empty message)
        => throw new NotImplementedException();

    public void SetRequestContext(ReverseCallRequestContext context, Empty request)
    {
        throw new NotImplementedException();
    }

    public ReverseCallResponseContext GetResponseContext(Empty message)
        => throw new NotImplementedException();

    public void SetPing(Empty message, Ping ping)
    {
        throw new NotImplementedException();
    }

    public Pong GetPong(Empty message)
        => throw new NotImplementedException();

    public Empty CreateFailedConnectResponse(FailureReason failureMessage)
        => throw new NotImplementedException();

    public Empty ConvertConnectArguments(Empty arguments)
        => throw new NotImplementedException();

    public ConnectArgumentsValidationResult ValidateConnectArguments(Empty arguments)
        => throw new NotImplementedException();
}