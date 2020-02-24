// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines a system that knows how to validate filters.
    /// </summary>
    public interface IFilterValidator
    {
        /// <summary>
        /// Validates a filter.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The async operation of validating a filter.</returns>
        Task Validate(StreamId sourceStream, StreamId targetStream, CancellationToken cancellationToken);

        /// <summary>
        /// Validates a filter.
        /// </summary>
        /// <param name="definition">The <see cref="IFilterDefinition" /> to validate against.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The async operation of validating a filter.</returns>
        Task Validate(IFilterDefinition definition, CancellationToken cancellationToken);
    }
}