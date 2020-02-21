// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a registry for <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    public interface ITypePartitionFilterRegistry
    {
        /// <summary>
        /// Registers a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
        /// </summary>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="definition">The <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <returns>The async operation of registering a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</returns>
        Task Register(StreamId targetStream, StreamId sourceStream, TypeFilterWithEventSourcePartitionDefinition definition);

        /// <summary>
        /// Removes a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
        /// </summary>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <returns>The async operation of removing a persisted <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</returns>
        Task Remove(StreamId targetStream, StreamId sourceStream);

        /// <summary>
        /// Validates a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
        /// </summary>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="definition">The <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <returns>The async operation of validating a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</returns>
        Task Validate(StreamId targetStream, StreamId sourceStream, TypeFilterWithEventSourcePartitionDefinition definition);
    }
}