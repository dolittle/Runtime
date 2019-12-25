// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizon"/>.
    /// </summary>
    [Singleton]
    public class EventHorizon : IEventHorizon
    {
        readonly List<ISingularity> _singularities = new List<ISingularity>();
        readonly IExecutionContextManager _executionContextManager;
        readonly IFetchUnprocessedCommits _unprocessedCommitFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizon"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with <see cref="ExecutionContext"/>.</param>
        /// <param name="unprocessedCommitsFetcher"><see cref="IFetchUnprocessedCommits"/> for fetching unprocessed commits.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHorizon(IExecutionContextManager executionContextManager, IFetchUnprocessedCommits unprocessedCommitsFetcher, ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _unprocessedCommitFetcher = unprocessedCommitsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<ISingularity> Singularities
        {
            get
            {
                lock (_singularities) return _singularities;
            }
        }

        /// <inheritdoc/>
        public void PassThrough(Processing.CommittedEventStreamWithContext committedEventStream)
        {
            lock (_singularities)
            {
                _logger.Information($"Committed eventstream entering event horizon with {_singularities.Count} singularities");
                _singularities
                    .Where(_ => _.CanPassThrough(committedEventStream)).AsParallel()
                    .ForEach(_ =>
                    {
                        _logger.Information($"Passing committed eventstream through singularity identified with bounded context '{_.BoundedContext}' in application '{_.Application}'");
                        _.PassThrough(committedEventStream);
                    });
            }
        }

        /// <inheritdoc/>
        public void Collapse(ISingularity singularity)
        {
            lock (_singularities)
            {
                _logger.Information($"Quantum tunnel collapsed for singularity identified with bounded context '{singularity.BoundedContext}' in application '{singularity.Application}'");
                _singularities.Remove(singularity);
            }
        }

        /// <inheritdoc/>
        public void GravitateTowards(ISingularity singularity, IEnumerable<TenantOffset> tenantOffsets)
        {
            lock (_singularities)
            {
                _logger.Information($"Gravitate events in the event horizon towards singularity identified with bounded context '{singularity.BoundedContext}' in application '{singularity.Application}'");
                _singularities.Add(singularity);
                SendUnprocessedCommitsThroughSingularity(tenantOffsets, singularity);
            }
        }

        void SendUnprocessedCommitsThroughSingularity(IEnumerable<TenantOffset> tenantOffsets, ISingularity singularity)
        {
            var commits = GetCommits(tenantOffsets);
            PassThroughSingularity(commits, singularity);
        }

        IEnumerable<Commits> GetCommits(IEnumerable<TenantOffset> tenantOffsets)
        {
            var commits = new List<Commits>();
            Parallel.ForEach(tenantOffsets, (_) =>
            {
                _executionContextManager.CurrentFor(_.Tenant);
                commits.Add(_unprocessedCommitFetcher.GetUnprocessedCommits(_.Offset));
            });
            return commits;
        }

        void PassThroughSingularity(IEnumerable<Commits> commitsArray, ISingularity singularity)
        {
            commitsArray.ForEach(commits =>
            {
                commits.ForEach(commit =>
                {
                    var committedEventStreamWithContext = new Processing.CommittedEventStreamWithContext(commit, commit.Events.First().Metadata.OriginalContext.ToExecutionContext(commit.CorrelationId));
                    if (singularity.CanPassThrough(committedEventStreamWithContext)) singularity.PassThrough(committedEventStreamWithContext);
                });
            });
        }
    }
}