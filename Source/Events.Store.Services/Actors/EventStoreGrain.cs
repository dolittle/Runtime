// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using Artifact = Dolittle.Runtime.Artifacts.Artifact;

namespace Dolittle.Runtime.Events.Store.Services.Actors;

[TenantGrain(typeof(EventStoreGrainActor), typeof(EventStoreGrainClient))]
public class EventStoreGrain : EventStoreGrainBase
{
    readonly ClusterIdentity _identity;
    readonly IEventStore _eventStore;
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly ILogger _logger;

    public EventStoreGrain(IContext context, ClusterIdentity identity, IEventStore eventStore, ICreateExecutionContexts executionContextCreator, ILogger logger)
        : base(context)
    {
        _identity = identity;
        _eventStore = eventStore;
        _executionContextCreator = executionContextCreator;
        _logger = logger;
    }

    public override async Task<CommitEventsResponse> Commit(Commit request)
    {
        var result =  await _executionContextCreator
            .TryCreateUsing(request.ExecutionContext.ToExecutionContext())
            .Then(async executionContext =>
            {
                var response = new Contracts.CommitEventsResponse();
                var events = request.Events.Select(_ => new UncommittedEvent(_.EventSourceId, new Artifact(_.EventType.Id.ToGuid(), _.EventType.Generation), _.Public, _.Content));
                var res = await _eventStore.CommitEvents(new UncommittedEvents(new ReadOnlyCollection<UncommittedEvent>(events.ToList())), request.ExecutionContext.ToExecutionContext(), Context.CancellationToken).ConfigureAwait(false);
                response.Events.AddRange(res.ToProtobuf());
                return response;
            })
            .Then(_ =>
                Log.EventsSuccessfullyCommitted(_logger))
            .Catch(exception =>
            {
                Log.ErrorCommittingEvents(_logger, exception);
            });
        return result.Success ? result : new CommitEventsResponse
        {
            Failure = result.Exception.ToFailure()
        };
    }
}
