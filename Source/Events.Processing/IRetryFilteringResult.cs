// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines the filtering result for when filtering should be tried again.
    /// </summary>
    public interface IRetryFilteringResult : IRetryProcessingResult, IFilterResult
    {
    }
}