// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Exception that gets thrown when there are no events in the <see cref="UncommittedEvents" /> sequence when it is being committed.
/// </summary>
public class NoEventsToCommit : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoEventsToCommit"/> class.
    /// </summary>
    public NoEventsToCommit()
        : base("There are no events in uncommitted events sequence")
    {
    }
}