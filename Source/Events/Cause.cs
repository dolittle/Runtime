// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;
using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents a link to an instance of a cause that caused <see cref="IEvent"/>s to occur.
    /// </summary>
    public class Cause : Value<Cause>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cause"/> class.
        /// </summary>
        /// <param name="type">The <see cref="CauseType"/> of the <see cref="Cause"/>.</param>
        /// <param name="position">The position in the corresponding append-only log where the instance of the cause is located.</param>
        public Cause(CauseType type, CauseLogPosition position)
        {
            Type = type;
            Position = position;
        }

        /// <summary>
        /// Gets the <see cref="CauseType"/> of the <see cref="Cause"/>.
        /// </summary>
        public CauseType Type { get; }

        /// <summary>
        /// Gets the position in the corresponding append-only log where the instance of the cause is located.
        /// </summary>
        public CauseLogPosition Position { get; }
    }
}
