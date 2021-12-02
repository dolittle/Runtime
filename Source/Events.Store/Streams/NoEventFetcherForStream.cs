// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Exception that gets thrown when there are no instance of <see cref="ICanFetchEventsFromStream" /> for the <see cref="IStreamDefinition" />.
/// </summary>
public class NoEventFetcherForStream : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoEventFetcherForStream"/> class.
    /// </summary>
    /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
    public NoEventFetcherForStream(IStreamDefinition streamDefinition)
        : base($"Could not find an events fetcher for Stream: {streamDefinition}")
    {
    }
}