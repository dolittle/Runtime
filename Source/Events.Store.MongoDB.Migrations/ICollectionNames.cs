// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public interface ICollectionNames
    {
        IEnumerable<string> Aggregates { get; }
        IEnumerable<string> EventLogs { get; }
        IEnumerable<string> EventStreams { get; }
        IEnumerable<string> SubscriptionStates { get; }
    }
}