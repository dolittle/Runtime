// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.Services.Grpc;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using event_store_given = Integration.Tests.Events.Store.given;
using EventStore = Dolittle.Runtime.Events.Contracts.EventStore;

namespace Integration.Tests.Events.Store.Services.Grpc.given;

class a_clean_event_store_and_grpc_service : event_store_given.a_clean_event_store
{
    protected static EventStore.EventStoreBase event_store_service;

    protected static IList<FetchForAggregateResponse> fetch_for_aggregate_written_responses;
    protected static IServerStreamWriter<FetchForAggregateResponse> fetch_for_aggregate_response_stream;

    protected static ServerCallContext server_call_context;

    Establish context = () =>
    {
        event_store_service = runtime.Host.Services.GetRequiredService<EventStoreGrpcService>();

        fetch_for_aggregate_written_responses = new List<FetchForAggregateResponse>();
        var fetch_for_aggregate_response_stream_mock = new Mock<IServerStreamWriter<FetchForAggregateResponse>>();
        fetch_for_aggregate_response_stream_mock
            .Setup(_ => _.WriteAsync(Moq.It.IsAny<FetchForAggregateResponse>(), Moq.It.IsAny<CancellationToken>()))
            .Callback((FetchForAggregateResponse request, CancellationToken cancellation_token) =>
            {
                fetch_for_aggregate_written_responses.Add(request);
            })
            .Returns(Task.CompletedTask);

        fetch_for_aggregate_response_stream = fetch_for_aggregate_response_stream_mock.Object;

        server_call_context = Mock.Of<ServerCallContext>();
    };
}