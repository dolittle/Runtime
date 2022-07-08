// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Exception that gets thrown when a stream processor state does not exist.
/// </summary>
public class StreamProcessorStateDoesNotExist : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorStateDoesNotExist" /> class.
    /// </summary>
    /// <param name="streamProcessor">The stream processor identifier.</param>
    public StreamProcessorStateDoesNotExist(IStreamProcessorId streamProcessor)
        : base($"Stream processor {streamProcessor} does not exist")
    {
    }
}