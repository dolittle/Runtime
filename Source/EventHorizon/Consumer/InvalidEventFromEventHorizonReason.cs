// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents the reason for why an event from an event horizon is invalid.
    /// </summary>
    public class InvalidEventFromEventHorizonReason : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="InvalidEventFromEventHorizonReason"/>.
        /// </summary>
        /// <param name="reason"><see cref="string"/> representation.</param>
        public static implicit operator InvalidEventFromEventHorizonReason(string reason) => new InvalidEventFromEventHorizonReason { Value = reason };
    }
}