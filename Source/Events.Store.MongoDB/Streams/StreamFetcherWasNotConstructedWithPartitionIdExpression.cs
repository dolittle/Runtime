// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Exception that gets thrown when invoking methods that filter on ParitionId and the <see cref="StreamFetcher{TEvent}"/> was constructed without a ParitionId expression.
/// </summary>
public class StreamFetcherWasNotConstructedWithPartitionIdExpression : Exception
{
    public StreamFetcherWasNotConstructedWithPartitionIdExpression()
        : base($"The StreamFetcher was constructed without a ParitionId expression. It cannot be used for fetching events or types in a specific partition")
    {
    }
}