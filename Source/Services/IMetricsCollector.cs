// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Defines a system for collecting metrics about services.
/// </summary>
public interface IMetricsCollector
{
    /// <summary>
    /// Adds to the total time spent writing request and waiting for response.
    /// </summary>
    /// <param name="requestTime">The time spent writing and getting response from request.</param>
    void AddToTotalRequestTime(TimeSpan requestTime);
    
    /// <summary>
    /// Adds to the total amount of requests.
    /// </summary>
    void AddRequest();
    
    /// <summary>
    /// Adds to the total amount of failed requests.
    /// </summary>
    void AddFailedRequest();
}
