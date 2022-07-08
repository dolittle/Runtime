// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Exception that gets thrown when attempting to register a Stream Processor which has already been registered.
/// </summary>
public class StreamProcessorAlreadyRegistered : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorAlreadyRegistered"/> class.
    /// </summary>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    public StreamProcessorAlreadyRegistered(IStreamProcessorId streamProcessorId)
        : base($"Stream Processor: '{streamProcessorId}' is already initialized")
    {
    }
}