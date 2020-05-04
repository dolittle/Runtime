// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents the reason for why a <see cref="StreamPositionRange" /> is invalid.
    /// </summary>
    public class InvalidStreamPositionRangeReason : ConceptAs<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStreamPositionRangeReason"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public InvalidStreamPositionRangeReason(string reason) => Value = reason;

        /// <summary>
        /// Implicitly convert <see cref="string" /> to <see cref="InvalidStreamPositionRangeReason" />.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public static implicit operator InvalidStreamPositionRangeReason(string reason) => new InvalidStreamPositionRangeReason(reason);
    }
}