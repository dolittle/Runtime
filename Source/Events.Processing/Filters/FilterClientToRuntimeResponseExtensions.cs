// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Extension methods for <see cref="FilterClientToRuntimeResponse" />.
    /// </summary>
    public static class FilterClientToRuntimeResponseExtensions
    {
        /// <summary>
        /// Converts a <see cref="FilterClientToRuntimeResponse" /> to <see cref="IFilterResult" />.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The converted <see cref="IFilterResult" />.</returns>
        public static IFilterResult ToFilterResult(this FilterClientToRuntimeResponse response)
        {
            if (!response.Succeeded && !response.Retry) return new FailedFilteringResult(response.FailureReason ?? string.Empty);
            else if (!response.Succeeded && response.Retry) return new RetryFilteringResult(response.RetryTimeout, response.FailureReason ?? string.Empty);
            return new SucceededFilteringResult(response.IsIncluded, response.Partition.ToGuid());
        }
    }
}