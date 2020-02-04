// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the identification of a filter.
    /// </summary>
    public class FilterId : ConceptAs<Guid>
    {
        /// <summary>
        /// Represents a not set <see cref="FilterId"/>.
        /// </summary>
        public static FilterId NotSet = Guid.Empty;

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to a <see cref="FilterId"/>.
        /// </summary>
        /// <param name="filterId"><see cref="Guid"/> representation.</param>
        public static implicit operator FilterId(Guid filterId) => new FilterId { Value = filterId };

        /// <summary>
        /// Creates a new instance of <see cref="FilterId"/> with a unique id.
        /// </summary>
        /// <returns>A new <see cref="FilterId"/>.</returns>
        public static FilterId New()
        {
            return new FilterId { Value = Guid.NewGuid() };
        }
    }
}
