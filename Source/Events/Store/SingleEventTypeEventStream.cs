// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// An enumerable of <see cref="CommittedEventEnvelope" />s for holding committed events that cross commits.
    /// </summary>
    public class SingleEventTypeEventStream : IEnumerable<CommittedEventEnvelope>, IEquatable<SingleEventTypeEventStream>
    {
        static readonly EnumerableEqualityComparer<CommittedEventEnvelope> _comparer = new EnumerableEqualityComparer<CommittedEventEnvelope>();
        readonly List<CommittedEventEnvelope> _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleEventTypeEventStream"/> class.
        /// </summary>
        /// <param name="events"><see cref="IEnumerable{T}"/> of <see cref="CommittedEventEnvelope"/>.</param>
        /// <exception type="MultipleEventTypesInSingleEventTypeEventStream">Thrown when there are event instances of more than one <see cref="ArtifactId">type</see>.</exception>
        public SingleEventTypeEventStream(IEnumerable<CommittedEventEnvelope> events)
        {
            if (events?.Any() == true)
            {
                var artifact = events.First().Metadata.Artifact.Id;
                if (!events.All(e => e.Metadata.Artifact.Id == artifact))
                    throw new MultipleEventTypesInSingleEventTypeEventStream();
            }

            _events = events != null ? new List<CommittedEventEnvelope>(events) : new List<CommittedEventEnvelope>();
        }

        /// <summary>
        /// Gets a value indicating whether or not there are any events.
        /// </summary>
        public bool IsEmpty => _events.Count == 0;

        /// <summary>
        /// Equates two <see cref="SingleEventTypeEventStream" /> instances.
        /// </summary>
        /// <param name="left">The left <see cref="SingleEventTypeEventStream" /> instance.</param>
        /// <param name="right">The right <see cref="SingleEventTypeEventStream" /> instance.</param>
        /// <returns>true if equal, otherwise false.</returns>
        public static bool Equals(SingleEventTypeEventStream left, SingleEventTypeEventStream right)
        {
            return _comparer.Equals(left, right);
        }

        /// <inheritdoc/>
        public bool Equals(SingleEventTypeEventStream other)
        {
            return Equals(this, other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as SingleEventTypeEventStream);
        }

        /// <inheritdoc/>
        public IEnumerator<CommittedEventEnvelope> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _comparer.GetHashCode(this);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _events.GetEnumerator() as IEnumerator;
        }
    }
}