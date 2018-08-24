using System;
using System.Collections;
using Dolittle.Concepts;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{

    /// <summary>
    /// A global sequence number for a specific event consisting of the Major (<see cref="CommitSequenceNumber" />), Minor (<see cref="EventSourceVersion" /> Commit) and Revision (<see cref="EventSourceVersion" /> Sequence)
    /// </summary>
    public class CommittedEventVersion : Value<CommittedEventVersion>, IComparable<CommittedEventVersion>
    {
        /// <summary>
        /// Indicates the Major number (<see cref="CommitSequenceNumber" />) associated with this event
        /// </summary>
        public ulong Major { get; }
        /// <summary>
        /// Indicates the Minor number (<see cref="EventSourceVersion" /> Commit) associated with this event
        /// </summary>
        public ulong Minor { get; }
        /// <summary>
        /// Indicates the Revision number (<see cref="EventSourceVersion" /> Sequence) associated with this event
        /// </summary>
        public uint Revision { get; }

        /// <summary>
        /// Instantiates a new instance of <see cref="CommittedEventVersion" />
        /// </summary>
        /// <param name="major">The Major number</param>
        /// <param name="minor">The Minor number</param>
        /// <param name="revision">The Revision number</param>
        public CommittedEventVersion(ulong major, ulong minor, uint revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        /// <summary>
        /// Compares two instances of <see cref="CommittedEventVersion" />
        /// </summary>
        /// <param name="other">The <see cref="CommittedEventVersion" /> to compare to</param>
        /// <returns>1 if this is greater, 0 if both are equal, -1 if the other is greater</returns>
        public int CompareTo(CommittedEventVersion other)
        {
            if (other == null) 
                return 1;

            if(this.Major == other.Major && this.Minor == other.Minor && this.Revision == other.Revision)
                return 0;
            
            if(this.Major > other.Major 
                || (this.Major == other.Major && this.Minor > other.Minor)
                || (this.Major == other.Major && this.Minor == other.Minor && this.Revision > other.Revision))
                return 1;

            return -1;
        }

        /// <summary>
        /// Indicates if the first version is greater than the second version
        /// </summary>
        /// <param name="first">First</param>
        /// <param name="second">Second</param>
        /// <returns>true if greater, false otherwise</returns>
        public static bool operator >  (CommittedEventVersion first, CommittedEventVersion second)
        {
            return first.CompareTo(second) == 1;
        }
        
        /// <summary>
        /// Indicates if the first version is less than the second version
        /// </summary>
        /// <param name="first">First</param>
        /// <param name="second">Second</param>
        /// <returns>true if less, false otherwise</returns>        
        public static bool operator <  (CommittedEventVersion first, CommittedEventVersion second)
        {
            return first.CompareTo(second) == -1;
        }

        /// <summary>
        /// Indicates if the first version is greater than or equal to the second version
        /// </summary>
        /// <param name="first">First</param>
        /// <param name="second">Second</param>
        /// <returns>true if greater than or equal to, false otherwise</returns>  
        public static bool operator >=  (CommittedEventVersion first, CommittedEventVersion second)
        {
            return first.CompareTo(second) >= 0;
        }
        
        /// <summary>
        /// Indicates if the first version is less than or equal to the second version
        /// </summary>
        /// <param name="first">First</param>
        /// <param name="second">Second</param>
        /// <returns>true if less than or equal to, false otherwise</returns>          
        public static bool operator <=  (CommittedEventVersion first, CommittedEventVersion second)
        {
            return first.CompareTo(second) <= 0;
        }

        /// <summary>
        /// Converts this to an <see cref="EventSourceVersion" />
        /// </summary>
        /// <returns>An <see cref="EventSourceVersion" /> equivalent</returns>
        public EventSourceVersion ToEventSourceVersion()
        {
            return new EventSourceVersion(this.Minor,this.Revision);
        }
    }
}