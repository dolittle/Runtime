// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Polly;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Defines a system that knows about policies used in <see cref="Subscription"/>.
/// </summary>
public interface ISubscriptionPolicies
{
    /// <summary>
    /// The policy to use for connecting a subscription
    /// </summary>
    IAsyncPolicy Connecting { get; }
}
