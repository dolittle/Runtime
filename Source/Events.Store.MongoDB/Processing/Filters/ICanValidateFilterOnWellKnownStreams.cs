// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Defines a filter validator that can validate filters that is based off well known streams.
    /// </summary>
    public interface ICanValidateFilterOnWellKnownStreams
    {
        /// <summary>
        /// Gets the well-known streams it can validate filters based off of.
        /// </summary>
        IEnumerable<StreamId> WellKnownStreams { get; }

        /// <summary>
        /// Gets a value indicating Whether the validator can validate filter on given stream.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <returns>Whether it can validate filter on stream.</returns>
        bool CanValidateFilterOn(StreamId sourceStream);

        /// <summary>
        /// Validates the filter.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="filter">The <see cref="AbstractFilterProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task of validating filter.</returns>
        Task Validate(StreamId sourceStream, StreamId targetStream, AbstractFilterProcessor filter, CancellationToken cancellationToken);
    }
}