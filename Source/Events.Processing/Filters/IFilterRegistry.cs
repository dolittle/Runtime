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
    public interface IFilterRegistry
    {
        /// <summary>
        /// Registers and validates a <see cref="IFilterProcessor{TDefinition}" />.
        /// </summary>
        /// <typeparam name="TDefinition">The subtype of <see cref="IFilterDefinition" />.</typeparam>
        /// <param name="filter">The <see cref="IFilterProcessor{TDefinition}" /> filter.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task of registering the filter.</returns>
        Task Register<TDefinition>(IFilterProcessor<TDefinition> filter, CancellationToken cancellationToken = default)
            where TDefinition : IFilterDefinition;

        /// <summary>
        /// De registers the filter that targets the given <see cref="StreamId" />..
        /// </summary>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        void Unregister(StreamId targetStream);

        /// <summary>
        /// Removes the persisted filter if it is persisted.
        /// </summary>
        /// <param name="targetStream">The target <see cref="StreamId"/> of the filter to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task of removing a persisted filter.</returns>
        Task RemoveIfPersisted(StreamId targetStream, CancellationToken cancellationToken = default);
    }
}