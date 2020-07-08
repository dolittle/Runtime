// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the reason for why the validation of a <see cref="IFilterDefinition" /> failed.
    /// </summary>
    public class FailedFilterValidationReason : Concepts.ConceptAs<string>
    {
        /// <summary>
        /// Implicitly converts <see cref="string" /> to <see cref="FailedFilterValidationReason" />.
        /// </summary>
        /// <param name="reason">The value.</param>
        public static implicit operator FailedFilterValidationReason(string reason) => new FailedFilterValidationReason { Value = reason };
    }
}
