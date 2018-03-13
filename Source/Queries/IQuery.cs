/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.ReadModels;

namespace Dolittle.Queries
{
    /// <summary>
    /// Defines a query for an unspecified type, but typically an <see cref="IReadModel"/>.
    /// </summary>
    /// <remarks>
    /// Implementing types must define a Query property with a getter having a return type that is supported by
    /// an implementation of <see cref="IQueryProviderFor{T}"/>.
    /// </remarks>
    public interface IQuery
    {
    }
}
