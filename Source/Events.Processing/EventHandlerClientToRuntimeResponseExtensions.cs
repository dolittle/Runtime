// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Extension methods for <see cref="EventHandlerClientToRuntimeResponse" />.
    /// </summary>
    public static class EventHandlerClientToRuntimeResponseExtensions
    {
        /// <summary>
        /// Converts the <see cref="EventHandlerClientToRuntimeResponse" /> to a <see cref="IProcessingResult" />.
        /// </summary>
        /// <param name="response">The <see cref="EventHandlerClientToRuntimeResponse" />.</param>
        /// <returns>The converted <see cref="IProcessingResult" />.</returns>
        public static IProcessingResult ToProcessingResult(this EventHandlerClientToRuntimeResponse response)
        {
            if (!response.Succeeded && !response.Retry) return new FailedProcessingResult(response.FailureReason ?? string.Empty);
            else if (!response.Succeeded && response.Retry) return new RetryProcessingResult(response.RetryTimeout, response.FailureReason ?? string.Empty);
            return new SucceededProcessingResult();
        }
    }
}