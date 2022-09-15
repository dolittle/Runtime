// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Contracts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// The exception that gets thrown when a <see cref="FetchForAggregateInBatchesRequest"/> is received with an unknown request type.
/// </summary>
public class UnsupportedFetchForAggregatesInBatchesType : Exception
{
    /// <summary>
    /// Initialises a new instance of the <see cref="UnsupportedFetchForAggregatesInBatchesType"/> class.
    /// </summary>
    /// <param name="type">The request type that was sent.</param>
    public UnsupportedFetchForAggregatesInBatchesType(FetchForAggregateInBatchesRequest.RequestOneofCase type)
        : base($"The FetchForAggregatesInBatches request type {type} is not supported.")
    {
    }
}
