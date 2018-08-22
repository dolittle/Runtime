using System;
using Dolittle.Concepts;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Identifies a specific <see cref="IEventSource" /> along with the current version (commit, sequence)
    /// </summary>
    public class EventSourceVersion : Value<EventSourceVersion>, IComparable<EventSourceVersion>
    {
        /// <summary>
        /// Instantiates an instance of a <see cref="EventSourceVersion" />
        /// </summary>
        /// <param name="commit">The commit (major) version</param>
        /// <param name="sequence">The event sequence (minor) version</param>
        public EventSourceVersion(ulong commit, uint sequence)
        {
            Commit = commit;
            Sequence = sequence;
        }

        /// <summary>
        /// The commit (major) version
        /// </summary>
        /// <value></value>
        public ulong Commit { get; }
        /// <summary>
        /// The event sequence (minor) version
        /// </summary>
        /// <value></value>
        public uint Sequence { get; }

        /// <summary>
        /// Inidicates whether this in the initial version for this <see cref="IEventStore" />
        /// </summary>
        public bool IsInitial => Commit == 1 && Sequence == 0;

        /// <summary>
        /// A string representation of the <see cref="EventSourceVersion" />
        /// </summary>
        /// <returns>A string representation of the <see cref="EventSourceVersion" /></returns>
        public override string ToString() => $"{Commit}.{Sequence}";

        /// <summary>
        /// Increments the sequence number of the EventSourceVersion by one.
        /// </summary>
        /// <returns>A new instance of <see cref="EventSourceVersion" /> with an incremented sequence number</returns>
        public EventSourceVersion IncrementSequence()
        {
            if(this == null)
                return EventSourceVersion.Initial();

            return new EventSourceVersion(this.Commit, (this.Sequence + 1));
        }

        /// <summary>
        /// Returns an Initial version of the <see cref="IEventSource" /> with a Commit of 1 and a Sequence of 0
        /// </summary>
        /// <returns></returns>
        public static EventSourceVersion Initial()
        {
            return new EventSourceVersion(1, 0);
        }

        /// <summary>
        /// Compares this <see cref="EventSourceVersion" /> to another
        /// </summary>
        /// <param name="other"> The <see cref="EventSourceVersion" /> to compare to</param>
        /// <returns>-1 is this version is less than the compared version, 0 if both version are equal, 1 is this version is greater than the compared version</returns>
        public int CompareTo(EventSourceVersion other)
        {
            if(other == null)
                return 1;

            if(this.Commit > other.Commit)
                return 1;

            if(this.Commit < other.Commit)
                return -1;

            return this.Sequence > other.Sequence ? 1 : this.Sequence == other.Sequence ? 0 : -1;
        }
    }
}