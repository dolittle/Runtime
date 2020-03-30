// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterResult" />.
    /// </summary>
    public partial class FilteringResult : IFilterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringResult"/> class.
        /// </summary>
        /// <param name="isIncluded">Is event included in filter.</param>
        /// <param name="partitionId">The <see cref="PartitionId" /> />.</param>
        public FilteringResult(bool isIncluded, PartitionId partitionId)
        {
            IsIncluded = isIncluded;
            Partition = partitionId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringResult"/> class.
        /// </summary>
        /// <param name="failure">The <see cref="ProcessorFailure" />.</param>
        public FilteringResult(ProcessorFailure failure)
        {
            IsIncluded = false;
            Partition = PartitionId.NotSet;
            Failure = failure;
        }

        /// <inheritdoc />
        public bool Succeeded => Failure == null;

        /// <inheritdoc />
        public bool Retry => Failure?.Retry == true;

        #nullable enable
        /// <inheritdoc />
        public ProcessorFailure? Failure { get; }

        /// <inheritdoc />
        public bool IsIncluded { get; }

        /// <inheritdoc />
        public PartitionId Partition { get; }

        /// <summary>
        /// Implicitly converts the <see cref="FilterClientToRuntimeResponse" /> to <see cref="FilteringResult" />.
        /// </summary>
        /// <param name="response">The <see cref="FilterClientToRuntimeResponse" />.</param>
        public static implicit operator FilteringResult(FilterClientToRuntimeResponse response) =>
            response.Failed == null ?
                new FilteringResult(response.Success.IsIncluded, response.Success.Partition.To<PartitionId>())
                : new FilteringResult(response.Failed);
    }
}