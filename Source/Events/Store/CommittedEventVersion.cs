// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A global sequence number for a specific event consisting of the Major (<see cref="CommitSequenceNumber" />),
    /// Minor (<see cref="EventSourceVersion" /> Commit) and Revision (<see cref="EventSourceVersion" /> Sequence).
    /// </summary>
    public class CommittedEventVersion : Value<CommittedEventVersion>, IComparable<CommittedEventVersion>, IComparable
    {
        /// <summary>
        /// Represents no committed event.
        /// </summary>
        public static readonly CommittedEventVersion None = new CommittedEventVersion(0, 0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEventVersion"/> class.
        /// </summary>
        /// <param name="major">The Major number.</param>
        /// <param name="minor">The Minor number.</param>
        /// <param name="revision">The Revision number.</param>
        public CommittedEventVersion(ulong major, ulong minor, uint revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        /// <summary>
        /// Gets indicates the Major number (<see cref="CommitSequenceNumber" />) associated with this event.
        /// </summary>
        public ulong Major { get; }

        /// <summary>
        /// Gets indicates the Minor number (<see cref="EventSourceVersion" /> Commit) associated with this event.
        /// </summary>
        public ulong Minor { get; }

        /// <summary>
        /// Gets indicates the Revision number (<see cref="EventSourceVersion" /> Sequence) associated with this event.
        /// </summary>
        public uint Revision { get; }

        /// <summary>
        /// Indicates if the first version is greater than the second version.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if greater, false otherwise.</returns>
        public static bool operator >(CommittedEventVersion left, CommittedEventVersion right)
        {
            return left.CompareTo(right) == 1;
        }

        /// <summary>
        /// Indicates if the first version is less than the second version.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if less, false otherwise.</returns>
        public static bool operator <(CommittedEventVersion left, CommittedEventVersion right)
        {
            return left.CompareTo(right) == -1;
        }

        /// <summary>
        /// Indicates if the first version is greater than or equal to the second version.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if greater than or equal to, false otherwise.</returns>
        public static bool operator >=(CommittedEventVersion left, CommittedEventVersion right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Indicates if the first version is less than or equal to the second version.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if less than or equal to, false otherwise.</returns>
        public static bool operator <=(CommittedEventVersion left, CommittedEventVersion right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Implicit equality operator for comparing two <see cref="CommittedEventVersion"/> instances.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if equal, false if not.</returns>
        public static bool operator ==(CommittedEventVersion left, CommittedEventVersion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implicit not-equal operator for comparing two <see cref="CommittedEventVersion"/> instances.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>true if not equal, false if they are.</returns>
        public static bool operator !=(CommittedEventVersion left, CommittedEventVersion right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares two instances of <see cref="CommittedEventVersion" />.
        /// </summary>
        /// <param name="other">The <see cref="CommittedEventVersion" /> to compare to.</param>
        /// <returns>1 if this is greater, 0 if both are equal, -1 if the other is greater.</returns>
        public int CompareTo(CommittedEventVersion other)
        {
            if (other == null)
                return 1;

            if (Major == other.Major && Minor == other.Minor && Revision == other.Revision)
                return 0;

            if (Major > other.Major
                || (Major == other.Major && Minor > other.Minor)
                || (Major == other.Major && Minor == other.Minor && Revision > other.Revision))
            {
                return 1;
            }

            return -1;
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as CommittedEventVersion);
        }

        /// <summary>
        /// Converts this to an <see cref="EventSourceVersion" />.
        /// </summary>
        /// <returns>An <see cref="EventSourceVersion" /> equivalent.</returns>
        public EventSourceVersion ToEventSourceVersion()
        {
            return new EventSourceVersion(Minor, Revision);
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