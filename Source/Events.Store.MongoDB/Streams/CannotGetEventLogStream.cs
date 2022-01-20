// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Exception that gets thrown when an Event Log is attempted retrieved as a normal Stream.
/// </summary>
public class CannotGetEventLogStream : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotGetEventLogStream"/> class.
    /// </summary>
    public CannotGetEventLogStream()
        : base($"An Event Log cannot be retrieved as a normal Stream")
    {
    }
}