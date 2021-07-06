// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// A unique identifier to allow us to trace actions and their consequencies throughout the system.
    /// </summary>
    public record CorrelationId(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Represents an Empty <see cref="CorrelationId" />.
        /// </summary>
        public static readonly CorrelationId Empty = Guid.Empty;

        /// <summary>
        /// Represents the correlation used by the system.
        /// </summary>
        public static readonly CorrelationId System = Guid.Parse("868ff40f-a133-4d0f-bfdd-18d726181e01");


        /// <summary>
        /// Implicitly converts a <see cref="Guid" /> to an instance of <see cref="CorrelationId" />.
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="CorrelationId" /> with.</param>
        public static implicit operator CorrelationId(Guid value) => new(value);

        /// <summary>
        /// Implicitly converts a <see cref="string" /> to an instance of <see cref="CorrelationId" />.
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="CorrelationId" /> with.</param>
        public static implicit operator CorrelationId(string value) => new(Guid.Parse(value));

        /// <summary>
        /// Creates a new <see cref="CorrelationId" /> with a generated Guid value.
        /// </summary>
        /// <returns>A <see cref="CorrelationId" /> initialized with a random Guid value.</returns>
        public static CorrelationId New() => Guid.NewGuid();
    }
}