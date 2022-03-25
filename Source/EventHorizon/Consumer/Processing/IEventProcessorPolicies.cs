// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Polly;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Defines a system that knows about policies for <see cref="EventProcessor"/>.
/// </summary>
public interface IEventProcessorPolicies
{
    /// <summary>
    /// Gets the <see cref="IAsyncPolicy"/> for writing events from the event horizon.
    /// </summary>
    IAsyncPolicy WriteEvent { get; }
}
