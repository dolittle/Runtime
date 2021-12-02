// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents a failed <see cref="IFilterResult" />.
/// </summary>
public class FailedFiltering : FailedProcessing, IFilterResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedFiltering"/> class.
    /// </summary>
    /// <param name="reason">The reason for failure.</param>
    public FailedFiltering(string reason)
        : base(reason)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailedFiltering"/> class.
    /// </summary>
    /// <param name="reason">The reason for failure.</param>
    /// <param name="retry">Whether to retry processing.</param>
    /// <param name="retryTimeout">The retry timout <see cref="TimeSpan" />.</param>
    public FailedFiltering(string reason, bool retry, TimeSpan retryTimeout)
        : base(reason, retry, retryTimeout)
    {
    }

    /// <inheritdoc/>
    public bool IsIncluded => false;

    /// <inheritdoc/>
    public PartitionId Partition => PartitionId.None;

    /// <inheritdoc/>
    public bool IsPartitioned { get; }
}