// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Events.Runtime;
using Dolittle.Logging;
using Grpc.Core;
using static Dolittle.Events.Runtime.EventStore;
using grpc = Dolittle.Events.Runtime;

namespace Dolittle.Runtime.Events.Runtime
{
    /// <summary>
    /// Represents the implementation of.
    /// </summary>
    public class EventStoreService : EventStoreBase
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreService"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventStoreService(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<EventCommitResponse> Commit(grpc.UncommittedEvents request, ServerCallContext context)
        {
            _logger.Information($"Events received : {request.Events.Count}");
            return await Task.FromResult(new EventCommitResponse()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task<EventCommitResponse> CommitForAggregate(UncommittedAggregateEvents request, ServerCallContext context)
        {
            _logger.Information("Events for Aggregate received");
            return await Task.FromResult(new EventCommitResponse()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task<CommittedAggregateEvents> FetchForAggregate(Aggregate request, ServerCallContext context)
        {
            _logger.Information("Fetch for Aggregate");
            return await Task.FromResult(new CommittedAggregateEvents()).ConfigureAwait(false);
        }
    }
}