// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Polly;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Defines the policies to use for the <see cref="ResilientStreamProcessorStateRepository"/>.
/// </summary>
public interface IResilientStreamProcessorStateRepositoryPolicies
{
    /// <summary>
    /// The policy to use for persisting stream processor states.
    /// </summary>
    IAsyncPolicy Persisting { get; }
    
    /// <summary>
    /// The policy to use for getting stream processor states.
    /// </summary>
    IAsyncPolicy<Try<IStreamProcessorState>> Getting { get; }
}
