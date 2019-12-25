// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// An incrementing number used to identify the sequence in which a <see cref="CommittedEventStream" /> was committed to the Event Store.
    /// </summary>
    public class CommitSequenceNumber : ConceptAs<ulong>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommitSequenceNumber"/> class.
        /// </summary>
        /// <param name="value">Value representing.</param>
        public CommitSequenceNumber(ulong value) => Value = value;

        /// <summary>
        /// Implicitly convert from <see cref="ulong" /> to <see cref="CommitSequenceNumber" />.
        /// </summary>
        /// <param name="value">Sequence number as <see cref="ulong"/>.</param>
        public static implicit operator CommitSequenceNumber(ulong value) => new CommitSequenceNumber(value);
    }
}