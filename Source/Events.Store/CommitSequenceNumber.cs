using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// An incrementing number used to identify the sequence in which a <see cref="CommittedEventStream" /> was committed to the Event Stpre
    /// </summary>
    public class CommitSequenceNumber : ConceptAs<ulong>
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="CommitSequenceNumber" /> with the sequence number
        /// </summary>
        /// <param name="value"></param>
        public CommitSequenceNumber(ulong value) => Value = value;
    
        /// <summary>
        /// An implicit conversion from <see cref="ulong" /> to <see cref="CommitSequenceNumber" />
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator CommitSequenceNumber(ulong value) => new CommitSequenceNumber(value);
    }
}