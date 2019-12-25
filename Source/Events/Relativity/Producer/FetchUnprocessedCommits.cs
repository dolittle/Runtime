// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Implementation of <see cref="IFetchUnprocessedEvents" />.
    /// </summary>
    public class FetchUnprocessedCommits : IFetchUnprocessedCommits
    {
        readonly FactoryFor<IEventStore> _getEventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchUnprocessedCommits"/> class.
        /// </summary>
        /// <param name="getEventStore"><see cref="FactoryFor{T}"/> for <see cref="IEventStore"/>.</param>
        public FetchUnprocessedCommits(FactoryFor<IEventStore> getEventStore)
        {
            _getEventStore = getEventStore;
        }

        /// <inheritdoc />
        public Commits GetUnprocessedCommits(CommitSequenceNumber commitSequenceNumber)
        {
            List<CommittedEventStream> commits;
            List<CommittedEventStream> newCommits;

            using (var eventStore = _getEventStore())
            {
                commits = eventStore.FetchAllCommitsAfter(commitSequenceNumber).ToList();
            }

            if (commits.Count == 0) return new Commits(Array.Empty<CommittedEventStream>());

            do
            {
                newCommits = new List<CommittedEventStream>();
                var lastCommitSequenceNumber = commits.OrderByDescending(_ => _.Sequence).First().Sequence;

                using (var eventStore = _getEventStore())
                {
                    newCommits.AddRange(eventStore.FetchAllCommitsAfter(lastCommitSequenceNumber));
                }

                if (newCommits.Count > 0) commits.AddRange(newCommits);
            }
            while (newCommits.Count > 0);

            return new Commits(commits);
        }
    }
}