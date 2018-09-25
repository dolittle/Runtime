using System.Collections.Generic;
using System.Linq;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Implemenation of <see cref="IFetchUnprocessedEvents" />
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
            Commits commits;
            Commits newCommits;
            using(var eventStore = _getEventStore())
            {
                commits = eventStore.FetchAllCommitsAfter(commitSequenceNumber);
            }
            do
            {
                if(!commits.IsEmpty)
                {
                    using(var eventStore = _getEventStore())
                    {
                        newCommits = eventStore.FetchAllCommitsAfter(commitSequenceNumber);
                    }
                    var list = new List<CommittedEventStream>(commits.ToList());
                    list.AddRange(newCommits.ToList());
                }
            }
            while(!commits.IsEmpty);

            return commits;
        }
    }
}