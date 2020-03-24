// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Extension methods for <see cref="ScopedFilterClientToRuntimeResponse" />.
    /// </summary>
    public static class ScopedFilterClientToRuntimeResponseExtensions
    {
        /// <summary>
        /// Converts the <see cref="ScopedFilterClientToRuntimeResponse" /> to a <see cref="IProcessingResult" />.
        /// </summary>
        /// <param name="response">The <see cref="ScopedFilterClientToRuntimeResponse" />.</param>
        /// <returns>The converted <see cref="IProcessingResult" />.</returns>
        public static IFilterResult ToFilterResult(this ScopedFilterClientToRuntimeResponse response)
        {
            if (!response.Succeeded && !response.Retry) return new FailedFilteringResult(response.FailureReason ?? string.Empty);
            else if (!response.Succeeded && response.Retry) return new RetryFilteringResult(response.RetryTimeout, response.FailureReason ?? string.Empty);
            return new SucceededFilteringResult(response.IsIncluded, response.Partition.ToGuid());
        }
    }
}