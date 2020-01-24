// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines the result for the <see cref="IRemoteFilterService" />.
    /// </summary>
    public interface IFilterResult : IProcessingResult
    {
        /// <summary>
        /// Gets a value indicating whether the event should be included in the stream.
        /// </summary>
        bool IsIncluded { get; }

        /// <summary>
        /// Gets the partition.
        /// </summary>
        int Partition { get; }
    }
}