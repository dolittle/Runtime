// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Defines a system that can get the time when a stream processor should retry processing for a specific <see cref="IStreamProcessorState"/>.
/// </summary>
/// <typeparam name="T">A <see cref="Type" /> of <see cref="IStreamProcessorState" />.</typeparam>
public interface ICanGetTimeToRetryFor<T>
    where T : class, IStreamProcessorState
{
    /// <summary>
    /// Tries to get the <see cref="TimeSpan" /> for when to retry processing. Outputs the maximum value <see cref="TimeSpan" /> if there is no retry time.
    /// </summary>
    /// <param name="streamProcessorState">The <typeparamref name="T"/>.</param>
    /// <param name="timeToRetry">The <see cref="TimeSpan" /> for when to retry processsing a stream processor.</param>
    /// <returns>A value indicating whether there is a retry time.</returns>
    bool TryGetTimespanToRetry(T streamProcessorState, out TimeSpan timeToRetry);
}