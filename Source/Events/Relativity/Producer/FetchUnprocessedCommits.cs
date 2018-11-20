using System.Collections.Generic;
using System.Linq;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Implementation of <see cref="IFetchUnprocessedEvents" />
    /// </summary>
    public class FetchUnprocessedCommits : IFetchUnprocessedCommits
    {
        FactoryFor<IEventStore> _getEventStore;

        /// <summary>
        /// Instantiates an instance of <see cref="FetchUnprocessedEvents" />
        /// </summary>
        /// <param name="getEventStore"></param>
        public FetchUnprocessedCommits(FactoryFor<IEventStore> getEventStore)
        {
            _getEventStore = getEventStore;
        }

        /// <inheritdoc />
        public Commits GetUnprocessedCommits(CommitSequenceNumber commitSequenceNumber)
        {
            List<CommittedEventStream> commits;
            List<CommittedEventStream> newCommits;

            using(var eventStore = _getEventStore())
            {
                commits = eventStore.FetchAllCommitsAfter(commitSequenceNumber).ToList();
            }

            if (! commits.Any()) return new Commits(new CommittedEventStream[]{});
            do
            {
                newCommits = new List<CommittedEventStream>();
                var lastCommitSequenceNumber = commits.OrderByDescending(_ => _.Sequence).First().Sequence; // This could potentially become an endless loop of fetching new commits if N is big enough

                using(var eventStore = _getEventStore())
                {
                    newCommits.AddRange(eventStore.FetchAllCommitsAfter(lastCommitSequenceNumber));
                }

                if (newCommits.Any()) commits.AddRange(newCommits); 
                
            }
            while(newCommits.Any());

            return new Commits(commits);
        }
    }
}