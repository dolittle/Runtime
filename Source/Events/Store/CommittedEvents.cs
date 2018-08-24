using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A read-only collection of <see cref="CommittedEventStream" />
    /// </summary>
    public class Commits : IEnumerable<CommittedEventStream>, IEnumerable
    {
        readonly ReadOnlyCollection<CommittedEventStream> _commits;
        /// <summary>
        /// Instantiates a new instance of <see cref="Commits" /> with 
        /// </summary>
        /// <param name="commits"></param>
        public Commits(IEnumerable<CommittedEventStream> commits)
        {
            _commits = new ReadOnlyCollection<CommittedEventStream>(commits.ToList() ?? new List<CommittedEventStream>());
        }

        /// <summary>
        /// Gets an enumerator for enumerating over the <see cref="CommittedEventStream" />
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CommittedEventStream> GetEnumerator()
        {
            return _commits.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _commits.GetEnumerator();
        }
    }
}