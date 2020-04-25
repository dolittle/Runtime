// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the reason for why the registration of an Event Processor failed.
    /// </summary>
    public class FailedEventProcessorRegistrationReason : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert <see cref="string" /> to <see cref="FailedEventProcessorRegistrationReason" />.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public static implicit operator FailedEventProcessorRegistrationReason(string reason) => new FailedEventProcessorRegistrationReason { Value = reason };
    }
}
