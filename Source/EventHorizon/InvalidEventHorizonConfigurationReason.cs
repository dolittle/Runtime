// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents the reason for why an event horizon configuration is invalid.
    /// </summary>
    public class InvalidEventHorizonConfigurationReason : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="InvalidEventHorizonConfigurationReason"/>.
        /// </summary>
        /// <param name="reason"><see cref="string"/> representation.</param>
        public static implicit operator InvalidEventHorizonConfigurationReason(string reason) =>
            new InvalidEventHorizonConfigurationReason { Value = reason };
    }
}