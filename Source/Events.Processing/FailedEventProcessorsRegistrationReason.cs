// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the reason for why the registration then Event Processors failed.
    /// </summary>
    public class FailedEventProcessorsRegistrationReason : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert <see cref="string" /> to <see cref="FailedEventProcessorsRegistrationReason" />.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public static implicit operator FailedEventProcessorsRegistrationReason(string reason) => new FailedEventProcessorsRegistrationReason { Value = reason };
    }
}
