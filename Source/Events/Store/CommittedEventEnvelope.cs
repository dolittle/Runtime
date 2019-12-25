// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.PropertyBags;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A combination of the <see cref="EventMetadata" /> and a <see cref="PropertyBag" /> that is the persisted version of an <see cref="IEvent" />
    /// with the <see cref="CommitSequenceNumber" />.
    /// </summary>
    public class CommittedEventEnvelope : Value<CommittedEventEnvelope>, IComparable<CommittedEventEnvelope>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEventEnvelope"/> class.
        /// </summary>
        /// <param name="commitSequence">The <see cref="CommitSequenceNumber"/>.</param>
        /// <param name="metadata">The <see cref="EventMetadata"/>.</param>
        /// <param name="event">The event as <see cref="PropertyBag"/>.</param>
        public CommittedEventEnvelope(
            CommitSequenceNumber commitSequence,
            EventMetadata metadata,
            PropertyBag @event)
        {
            Metadata = metadata;
            Event = @event;
            Version = metadata.VersionedEventSource.ToCommittedEventVersion(commitSequence);
        }

        /// <summary>
        /// Gets the <see cref="EventMetadata" /> associated with this persisted <see cref="IEvent" />.
        /// </summary>
        public EventMetadata Metadata { get; }

        /// <summary>
        /// Gets a <see cref="PropertyBag" /> of the values associated with this <see cref="IEvent" />.
        /// </summary>
        public PropertyBag Event { get; }

        /// <summary>
        /// Gets the <see cref="EventId" /> of this <see cref="IEvent" />.
        /// </summary>
        public EventId Id => Metadata.Id;

        /// <summary>
        /// Gets the <see cref="CommittedEventVersion" /> of this Event.
        /// </summary>
        public CommittedEventVersion Version { get; }

        /// <summary>
        /// Implicit equality operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if equal, otherwise false.</returns>
        public static bool operator ==(CommittedEventEnvelope left, CommittedEventEnvelope right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Implicit not-equals operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if not equal, otherwise false.</returns>
        public static bool operator !=(CommittedEventEnvelope left, CommittedEventEnvelope right) => !(left == right);

        /// <summary>
        /// Implicit less than operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if left hand side is less than right, otherwise false.</returns>
        public static bool operator <(CommittedEventEnvelope left, CommittedEventEnvelope right) => left is null ? right is object : left.CompareTo(right) < 0;

        /// <summary>
        /// Implicit less than or equal operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if left hand side is less than or equal to right, otherwise false.</returns>
        public static bool operator <=(CommittedEventEnvelope left, CommittedEventEnvelope right) => left is null || left.CompareTo(right) <= 0;

        /// <summary>
        /// Implicit greater than operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if left hand side is greater than right, otherwise false.</returns>
        public static bool operator >(CommittedEventEnvelope left, CommittedEventEnvelope right) => left is object && left.CompareTo(right) > 0;

        /// <summary>
        /// Implicit greater than or equal operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if left hand side is greater than or equal to right, otherwise false.</returns>
        public static bool operator >=(CommittedEventEnvelope left, CommittedEventEnvelope right) => left is null ? right is null : left.CompareTo(right) >= 0;

        /// <summary>
        /// Compares two <see cref="CommittedEventEnvelope" /> based on the <see cref="CommittedEventVersion" /> of each.
        /// </summary>
        /// <param name="other">The <see cref="CommittedEventEnvelope" /> to compare to.</param>
        /// <returns>1 if greater, 0 if equal, -1 if less than.</returns>
        public int CompareTo(CommittedEventEnvelope other)
        {
            if (other == null)
                return 1;

            if (Version == null && other.Version == null)
                return 0;

            if (Version == null && other.Version != null)
                return -1;

            return Version.CompareTo(other.Version);
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as CommittedEventEnvelope);
        }

        /// <summary>
        /// Convert a <see cref="CommittedEventEnvelope" /> to the corresponding <see cref="EventEnvelope" />.
        /// </summary>
        /// <returns>An <see cref="EventEnvelope" />.</returns>
        public EventEnvelope ToEventEnvelope()
        {
            return new EventEnvelope(Metadata, Event);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}