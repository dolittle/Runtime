// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Exception that gets thrown when getting a partitioned Fetcher for an unpartitioned Stream.
/// </summary>
public class CannotGetPartitionedFetcherForUnpartitionedStream : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotGetPartitionedFetcherForUnpartitionedStream"/> class.
    /// </summary>
    /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
    public CannotGetPartitionedFetcherForUnpartitionedStream(IStreamDefinition streamDefinition)
        : base($"Cannot get partitioned Fetcher from unpartitioned Stream Definition: '{streamDefinition}'")
    {
    }
}