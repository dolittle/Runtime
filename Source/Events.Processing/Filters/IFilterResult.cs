// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines the result for a filter.
    /// </summary>
    public interface IFilterResult : IProcessingResult
    {
        /// <summary>
        /// Gets a value indicating whether the event should be included in the stream.
        /// </summary>
        bool IsIncluded { get; }

        /// <summary>
        /// Gets the <see cref="PartitionId" />.
        /// </summary>
        PartitionId Partition { get; }
    }
}