// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.EventHorizon.Producer.Filter
{
    /// <summary>
    /// Extension methods for <see cref="PublicFilterClientToRuntimeResponse" />.
    /// </summary>
    public static class PublicFilterClientToRuntimeResponseExtensions
    {
        /// <summary>
        /// Converts a <see cref="PublicFilterClientToRuntimeResponse" /> to <see cref="IFilterResult" />.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The converted <see cref="IFilterResult" />.</returns>
        public static IFilterResult ToFilterResult(this PublicFilterClientToRuntimeResponse response)
        {
            if (!response.Succeeded && !response.Retry) return new FailedFilteringResult(response.FailureReason ?? string.Empty);
            else if (!response.Succeeded && response.Retry) return new RetryFilteringResult(response.RetryTimeout, response.FailureReason ?? string.Empty);
            return new SucceededFilteringResult(response.IsIncluded, response.Partition.ToGuid());
        }
    }
}