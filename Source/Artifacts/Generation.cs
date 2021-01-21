// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Artifacts
{
    /// <summary>
    /// Represents a particular stage in the evolution of an artifact, corresponding to a specific form.
    /// </summary>
    public class Generation : ConceptAs<uint>
    {
        /// <summary>
        /// A readonly static representation of the Initial <see cref="Generation" /> i.e. with a generation value of 0.
        /// </summary>
        public static readonly Generation Initial = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Generation"/> class.
        /// </summary>
        /// <remarks>
        /// It gets initialized with the <see cref="Initial"/> value.
        /// </remarks>
        public Generation() => Value = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Generation"/> class.
        /// </summary>
        /// <param name="value">The generation to initialize with.</param>
        public Generation(uint value) => Value = value;

        /// <summary>
        /// Implicit convertion from Uint to Generation.
        /// </summary>
        /// <param name="value">Value to initialize the <see cref="Generation" /> instance with.</param>
        public static implicit operator Generation(uint value) => new Generation(value);
    }
}