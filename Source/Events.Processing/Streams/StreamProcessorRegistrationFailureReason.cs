// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents the reason for why <see cref="IEventProcessorsRegistration.Register()" /> failed.
    /// </summary>
    public class StreamProcessorRegistrationFailureReason : Concepts.ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert <see cref="string" /> to <see cref="StreamProcessorRegistrationFailureReason" />.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public static implicit operator StreamProcessorRegistrationFailureReason(string reason) =>
            new StreamProcessorRegistrationFailureReason { Value = reason };
    }
}