// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the reason for why a <see cref="IFilterDefinition" >filter</see> could not be registered.
    /// </summary>
    public class CouldNotRegisterFilterReason : ConceptAs<string>
    {
        /// <summary>
        /// Converts the <see cref="string" /> to <see cref="CouldNotRegisterFilterReason" />.
        /// </summary>
        /// <param name="reason">The value.</param>
        public static implicit operator CouldNotRegisterFilterReason(string reason) => new CouldNotRegisterFilterReason { Value = reason };
    }
}
