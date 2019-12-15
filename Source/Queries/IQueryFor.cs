// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ReadModels;

namespace Dolittle.Queries
{
    /// <summary>
    /// Defines a query for a specified type of <see cref="IReadModel"/>.
    /// </summary>
    /// <typeparam name="T">The type to query.</typeparam>
    /// <remarks>
    /// Types inheriting from this interface will be picked up proxy generation, deserialized and dispatched to the
    /// correct instance of <see cref="IQueryProviderFor{T}"/>.
    /// </remarks>
    public interface IQueryFor<T> : IQuery
        where T : IReadModel
    {
    }
}
