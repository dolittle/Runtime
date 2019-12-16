// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A read-only collection of <see cref="CommittedEventStream" />.
    /// </summary>
    public class Commits : IEnumerable<CommittedEventStream>, IEnumerable
    {
        readonly ReadOnlyCollection<CommittedEventStream> _commits;

        /// <summary>
        /// Initializes a new instance of the <see cref="Commits"/> class.
        /// </summary>
        /// <param name="commits"><see cref="IEnumerable{T}"/> of <see cref="CommittedEventStream"/> with the actual commits.</param>
        public Commits(IEnumerable<CommittedEventStream> commits)
        {
            _commits = new ReadOnlyCollection<CommittedEventStream>(commits.ToList() ?? new List<CommittedEventStream>());
        }

        /// <summary>
        /// Gets a value indicating whether indicates if the Commits is empty or not.
        /// </summary>
        public bool IsEmpty => _commits.Count == 0;

        /// <inheritdoc/>
        public IEnumerator<CommittedEventStream> GetEnumerator()
        {
            return _commits.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _commits.GetEnumerator();
        }
    }
}