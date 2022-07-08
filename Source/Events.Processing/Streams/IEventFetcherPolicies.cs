// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Polly;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Defines the policies to use for <see cref="ICanFetchEventsFromStream"/>.
/// </summary>
public interface IEventFetcherPolicies
{
    /// <summary>
    /// The policy to use while fetching.
    /// </summary>
    IAsyncPolicy<Try<IEnumerable<StreamEvent>>> Fetching { get; }
}
