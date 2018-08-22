using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A unique identifier for a <see cref="CommittedEventStream" />
    /// </summary>
    public class CommitId : ConceptAs<Guid>
    {
        /// <summary>
        /// A static readonly instance representing an Empty <see cref="CommitId" />.null  Initialized with Guid.Empty
        /// </summary>
        public static readonly CommitId Empty = Guid.Empty;

        /// <summary>
        /// Instaniates an  instance of <see cref="CommitId" /> with a Guid.Empty value
        /// </summary>
        public CommitId()
        {
            Value = Guid.Empty;
        } 

        /// <summary>
        /// Instaniates an instance of <see cref="CommitId" /> with a unique id
        /// </summary>
        /// <param name="guid"></param>
        public CommitId(Guid guid)
        {
            Value = guid;
        } 

        /// <summary>
        /// Creates a new instance of <see cref="CommitId" /> with a generated unique id
        /// </summary>
        /// <returns></returns>
        public static CommitId New()
        {
            return new CommitId(Guid.NewGuid());
        }

        /// <summary>
        /// Implicitly convert from a System.Guid to a <see cref="CommitId"/>
        /// </summary>
        /// <param name="value">The System.Guid vALUE to convert</param>
        public static implicit operator CommitId(Guid value)
        {
            return new CommitId(value);
        }        
    }
}