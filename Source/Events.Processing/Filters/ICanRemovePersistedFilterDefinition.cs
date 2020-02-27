// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that can remove a presisted <see cref="IFilterDefinition" />.
    /// </summary>
    public interface ICanRemovePersistedFilterDefinition
    {
        /// <summary>
        /// Removes the persisted <see cref="IFilterDefinition" />.
        /// </summary>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task of removing a persisted filter.</returns>
        Task RemovePersistedFilter(StreamId targetStream, CancellationToken cancellationToken = default);
    }
}