// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Exception that gets thrown when Filter Validation failed and you cannot close a dispatcher that is accepted.
/// </summary>
public class FilterValidationFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterValidationFailed"/> class.
    /// </summary>
    /// <param name="filterId">The Id of the Filter.</param>
    /// <param name="reason">The <see cref="FailedFilterValidationReason" />.</param>
    public FilterValidationFailed(StreamId filterId, FailedFilterValidationReason reason)
        : base($"The Filter: '{filterId}' failed validation. {reason}")
    {
    }
}