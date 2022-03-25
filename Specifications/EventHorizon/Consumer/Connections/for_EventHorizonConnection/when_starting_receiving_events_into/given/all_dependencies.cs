using System;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Nito.AsyncEx;
using Dolittle.Runtime.Services.Clients;
using Dolittle.Runtime.EventHorizon.Contracts;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Dolittle.Artifacts.Contracts;
using Dolittle.Runtime.Execution;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnection.when_starting_receiving_events_into.given;

public class all_dependencies : for_EventHorizonConnection.given.all_dependencies
{
    protected static AsyncProducerConsumerQueue<StreamEvent> event_queue;
    protected static CancellationTokenSource cts;

    Establish context = () =>
    {
        cts = new CancellationTokenSource();
        event_queue = new AsyncProducerConsumerQueue<StreamEvent>();
    };

    protected static void SetupReverseCallClient(params ConsumerRequest[] requests)
        => reverse_call_client.Setup(_ => _.Handle(Moq.It.IsAny<ReverseCallHandler<ConsumerRequest, ConsumerResponse>>(), Moq.It.IsAny<CancellationToken>()))
            .Returns<ReverseCallHandler<ConsumerRequest, ConsumerResponse>, CancellationToken>((callback, token) => Task.Run(async () =>
            {
                var i = 0;
                while (!token.IsCancellationRequested)
                {
                    if (i < requests.Length)
                    {
                        await callback(requests[i], execution_context, token).ConfigureAwait(false);
                    }
                    await Task.Delay(10).ConfigureAwait(false);
                    i++;
                }
                event_queue.CompleteAdding();
            }));
    protected static ConsumerRequest CreateRequest()
    {
        var execution_context = new Dolittle.Execution.Contracts.ExecutionContext
        {
            MicroserviceId = Guid.Parse("a2002eca-502f-41ca-bcaa-59149f6d44e6").ToProtobuf(),
            CorrelationId = Guid.Parse("11f3f1c1-340b-4d08-8a3a-9dae16cb2dbb").ToProtobuf(),
            TenantId = Guid.Parse("0dfdd63a-570c-4d17-b9c0-db34f1cb5bbc").ToProtobuf(),
            Environment = "env",
            Version = Version.NotSet.ToProtobuf(),
        };
        execution_context.Claims.AddRange(Claims.Empty.ToProtobuf());
        return new ConsumerRequest
        {
            Event = new EventHorizonEvent
            {
                StreamSequenceNumber = 1,
                Event = new CommittedEvent
                {
                    Content = "content",
                    EventLogSequenceNumber = 4,
                    EventSourceId = "some event source",
                    External = true,
                    ExecutionContext = execution_context,
                    ExternalEventLogSequenceNumber = 6,
                    ExternalEventReceived = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                    Occurred = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                    Public = false,
                    EventType = new Artifact
                    {
                        Generation = 4,
                        Id = Guid.Parse("19ced99f-376e-413c-b368-05edfc2f1067").ToProtobuf()
                    }
                },
            }
        };
    }
}