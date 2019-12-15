// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.ReadModels;

namespace Dolittle.Queries
{
    /// <summary>
    /// Defines the filtering system for <see cref="IReadModel">ReadModels</see>.
    /// </summary>
    public interface IReadModelFilters
    {
        /// <summary>
        /// Filters an incoming <see cref="IEnumerable{IReadModel}"/>.
        /// </summary>
        /// <param name="readModels"><see cref="IEnumerable{IReadModel}">ReadModels</see> to filter.</param>
        /// <returns>Filtered <see cref="IEnumerable{IReadModel}">ReadModels</see>.</returns>
        IEnumerable<IReadModel> Filter(IEnumerable<IReadModel> readModels);
    }
}
