// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents the versioning for an event source.
    /// </summary>
    public class EventSourceVersion : Value<EventSourceVersion>, IComparable<EventSourceVersion>, IComparable
    {
        /// <summary>
        /// No Version version.
        /// </summary>
        public static readonly EventSourceVersion NoVersion = new EventSourceVersion(0, 0);

        /// <summary>
        /// Initial version.
        /// </summary>
        public static readonly EventSourceVersion Initial = new EventSourceVersion(1, 0);

        const float SequenceDivisor = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceVersion"/> class.
        /// </summary>
        /// <param name="commit">Commit part of the version (major).</param>
        /// <param name="sequence">Sequence part of the version, within the commit (minor).</param>
        public EventSourceVersion(ulong commit, uint sequence)
        {
            Commit = commit;
            Sequence = sequence;
        }

        /// <summary>
        /// Gets the commit number of the version.
        /// </summary>
        public ulong Commit { get; }

        /// <summary>
        /// Gets the sequence number of the version.
        /// </summary>
        public uint Sequence { get; }

        /// <summary>
        /// Implicit equality operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if equal, otherwise false.</returns>
        public static bool operator ==(EventSourceVersion left, EventSourceVersion right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Implicit not-equals operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if not equal, otherwise false.</returns>
        public static bool operator !=(EventSourceVersion left, EventSourceVersion right) => !(left == right);

        /// <summary>
        /// Implicit less than operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if left hand side is less than right, otherwise false.</returns>
        public static bool operator <(EventSourceVersion left, EventSourceVersion right) => left is null ? right is object : left.CompareTo(right) < 0;

        /// <summary>
        /// Implicit less than or equal operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if left hand side is less than or equal to right, otherwise false.</returns>
        public static bool operator <=(EventSourceVersion left, EventSourceVersion right) => left is null || left.CompareTo(right) <= 0;

        /// <summary>
        /// Implicit greater than operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if left hand side is greater than right, otherwise false.</returns>
        public static bool operator >(EventSourceVersion left, EventSourceVersion right) => left is object && left.CompareTo(right) > 0;

        /// <summary>
        /// Implicit greater than or equal operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if left hand side is greater than or equal to right, otherwise false.</returns>
        public static bool operator >=(EventSourceVersion left, EventSourceVersion right) => left is null ? right is null : left.CompareTo(right) >= 0;

        /// <summary>
        /// Creates an <see cref="EventSourceVersion"/> from a combined floating point.
        /// </summary>
        /// <param name="combined">Version number in a combined fashion.</param>
        /// <returns>New instance of <see cref="EventSourceVersion"/>.</returns>
        public static EventSourceVersion FromCombined(double combined)
        {
            var commit = (ulong)combined;
            var sequence = (uint)Math.Round((combined - (double)commit) * SequenceDivisor);
            return new EventSourceVersion(commit, sequence);
        }

        /// <summary>
        /// Increase the commit number and return a new version.
        /// </summary>
        /// <returns><see cref="EventSourceVersion"/> with the new version.</returns>
        public EventSourceVersion NextCommit()
        {
            var nextCommit = new EventSourceVersion(Commit + 1, 0);
            return nextCommit;
        }

        /// <summary>
        /// Increase the sequence number and return a new version.
        /// </summary>
        /// <returns><see cref="EventSourceVersion"/> with the new version.</returns>
        public EventSourceVersion NextSequence()
        {
            if (Commit < 1)
                throw new InvalidEventSourceVersion($"Cannot get the Next Sequence on Commit {Commit}");

            var nextSequence = new EventSourceVersion(Commit, Sequence + 1);
            return nextSequence;
        }

        /// <summary>
        /// Decrease the commit number and return a new version.
        /// </summary>
        /// <returns><see cref="EventSourceVersion"/> with the new version.</returns>
        public EventSourceVersion PreviousCommit()
        {
            if (Commit < 1)
                throw new InvalidEventSourceVersion($"Cannot get the Previous Commit of Commit {Commit}");

            if (Commit == 1)
                return NoVersion;

            return new EventSourceVersion(Commit - 1, 0);
        }

        /// <summary>
        /// Compare this version with another version.
        /// </summary>
        /// <param name="other">The other version to compare to.</param>
        /// <returns>
        /// Less than zero - this instance is less than the other version
        /// Zero - this instance is equal to the other version
        /// Greater than zero - this instance is greater than the other version.
        /// </returns>
        public int CompareTo(EventSourceVersion other)
        {
            var current = Combine();
            var otherVersion = other.Combine();
            return current.CompareTo(otherVersion);
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as EventSourceVersion);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as EventSourceVersion);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Combines the Major / Minor number of Commit and Sequence into a single floating point number
        /// where the Commit is before the decimal place and Sequence is after.
        /// </summary>
        /// <returns>Combined <see cref="double"/> with version info.</returns>
        public double Combine()
        {
            var majorNumber = (double)Commit;
            var minorNumber = (double)Sequence / SequenceDivisor;
            return majorNumber + minorNumber;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[ Version : {Commit}.{Sequence} ]";
        }

        /// <summary>
        /// Creates a <see cref="CommittedEventVersion" /> based upon this <see cref="EventSourceVersion" /> and the provided <see cref="CommitSequenceNumber" />.
        /// </summary>
        /// <param name="commitSequenceNumber">The <see cref="CommitSequenceNumber" >sequence number</see> for the commit.</param>
        /// <returns>The <see cref="CommittedEventVersion" /> based on this <see cref="EventSourceVersion" />.</returns>
        public CommittedEventVersion ToCommittedEventVersion(CommitSequenceNumber commitSequenceNumber)
        {
            return new CommittedEventVersion(commitSequenceNumber, Commit, Sequence);
        }
    }
}