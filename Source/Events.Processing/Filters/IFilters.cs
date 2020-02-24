// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that knows about filters.
    /// </summary>
    public interface IFilters
    {
        /// <summary>
        /// Registers a <see cref="IFilterDefinition" /> together with the actual <see cref="AbstractFilterProcessor" />.
        /// </summary>
        /// <param name="definition">The <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <param name="filter">The <see cref="AbstractFilterProcessor" /> filter.</param>
        /// <returns>The async operation of registering a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</returns>
        Task Register(IFilterDefinition definition, AbstractFilterProcessor filter);

        /// <summary>
        /// Removes a filter.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <returns>The async operation of removing a persisted <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</returns>
        Task Remove(StreamId sourceStream, StreamId targetStream);

        /// <summary>
        /// Validates a filter.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <returns>The async operation of validating a filter.</returns>
        Task Validate(StreamId sourceStream, StreamId targetStream);
    }
}