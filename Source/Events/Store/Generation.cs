using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    //TODO: this should not be part of the Event Store.  Generation is a more general concept.  Artifacts?

    /// <summary>
    /// Represents a particular stage in the evolution of an artifact, corresponding to a specific form
    /// </summary>
    public class Generation : ConceptAs<uint>
    {
        /// <summary>
        /// A readonly static representation of the Initial <see cref="Generation" /> i.e. with a generation value of 0.
        /// </summary>
        public static readonly Generation Initial = 0;

        /// <summary>
        /// Instantiates a <see cref="Generation" /> with a value of 0
        /// </summary>
        public Generation() => Value = 0;
        /// <summary>
        /// Instantiates a <see cref="Generation" /> with the supplied generation
        /// </summary>
        /// <param name="value">The generation to initialize</param>
        public Generation(uint value) => Value = value;

        /// <summary>
        /// Implicit convertion from Uint to Generation
        /// </summary>
        /// <param name="value">Value to initialize the <see cref="Generation" /> instance with</param>
        public static implicit operator Generation(uint value) => new Generation(value);
    }
}