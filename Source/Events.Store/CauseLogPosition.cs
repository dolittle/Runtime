// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a position of a <see cref="Cause"/> in a log as a natural number.
    /// </summary>
    public class CauseLogPosition : ConceptAs<ulong>
    {
        /// <summary>
        /// Implicitly convert a <see cref="ulong"/> to an <see cref="CauseLogPosition"/>.
        /// </summary>
        /// <param name="number">The number.</param>
        public static implicit operator CauseLogPosition(ulong number) => new CauseLogPosition { Value = number };
    }
}