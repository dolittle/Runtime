// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Exception that gets thrown when the stored stream events is not ordered correctly.
/// </summary>
public class StoredStreamEventsHeadIsNotOrdered : Exception
{
    public StoredStreamEventsHeadIsNotOrdered()
        : base("The stream of stored events was expected to have event log sequence number in ascending order")
    {
    }
}
