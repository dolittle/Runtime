// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the reason for why the registration of a filter failed.
    /// </summary>
    public class FailedFilterRegistrationReason : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert <see cref="string" /> to <see cref="FailedFilterRegistrationReason" />.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public static implicit operator FailedFilterRegistrationReason(string reason) => new FailedFilterRegistrationReason { Value = reason };
    }
}
