using System;
using Dolittle.Concepts;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{

    /// <summary>
    /// A sequential number indicating the order in which commits were made for an <see cref="IEventSource" /> 
    /// </summary>
    public class CommitVersion : ConceptAs<ulong>
    {
        /// <summary>
        /// A static readonly instance to represent an Empty <see cref="CommitVersion" /> i.e. the version is 0.
        /// </summary>
        public static readonly CommitVersion Empty = 0;
        /// <summary>
        /// Instantiates a new instance of <see cref="CommitVersion" /> initialized with the provided value
        /// </summary>
        /// <param name="value">The value to initialize with</param>
        public CommitVersion(ulong value) => Value = value;
    
        /// <summary>
        /// An implicit conversion from the ulong value to an instance of <see cref="CommitVersion" /> initialized with the ulong value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator CommitVersion(ulong value) => new CommitVersion(value);
    }
}