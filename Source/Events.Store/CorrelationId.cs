using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    //TODO:  this replaces TransactionCorrelationId, find the correct place for it

    /// <summary>
    /// A unique identifier to allow us to trace actions and their consequencies throughout the system
    /// </summary>
    public class CorrelationId : ConceptAs<Guid>
    {
       /// <summary>
       /// Instantiates a <see cref="CorrelationId" /> with the specified value
       /// </summary>
       /// <param name="guid">The value to initialize the <see cref="CorrelationId" /> with</param>
        public CorrelationId(Guid guid) => Value = guid;
    
        /// <summary>
        /// Creates a new <see cref="CorrelationId" /> with a generated Guid value.
        /// </summary>
        /// <returns>A <see cref="CorrelationId" /> initialised with a random Guid value</returns>
        public static CorrelationId New() => new CorrelationId(Guid.NewGuid());
    
        /// <summary>
        /// Implicitly converts a <see cref="Guid" /> to an instance of <see cref="CorrelationId" />
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="CorrelationId" /> with</param>
        public static implicit operator CorrelationId(Guid value) => new CorrelationId(value);
    }
}