// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
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
        /// Registers and validates a <see cref="IFilterDefinition" /> together with the actual <see cref="AbstractFilterProcessor" />.
        /// </summary>
        /// <param name="definition">The <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <param name="filter">The <see cref="AbstractFilterProcessor" /> filter.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task of registering the filter.</returns>
        Task Register(IFilterDefinition definition, AbstractFilterProcessor filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a filter.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        void Remove(StreamId sourceStream, StreamId targetStream);

        /// <summary>
        /// Gets the <see cref="AbstractFilterProcessor" /> for a for a <see cref="StreamId"/>.
        /// </summary>
        /// <param name="targetStream">The <see cref="StreamId" />.</param>
        /// <returns>The filter.</returns>
        AbstractFilterProcessor GetFilterProcessorFor(StreamId targetStream);
    }
}