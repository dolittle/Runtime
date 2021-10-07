// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public class CollectionNames : ICollectionNames
    {
        readonly IEnumerable<string> _collectionNames;
        public CollectionNames(IEnumerable<string> collectionNames)
        {
            _collectionNames = collectionNames;
            Aggregates = GetAggregates();
            EventLogs = GetEventLogs();
            EventStreams = GetEventStreams();
            SubscriptionStates = GetSubscriptionStates();
        }
        public IEnumerable<string> Aggregates { get; }
        public IEnumerable<string> EventLogs { get; }
        public IEnumerable<string> EventStreams { get; }
        public IEnumerable<string> SubscriptionStates { get; }

        static bool IsScopedEventStream(string collectionName)
            => collectionName.Contains("x-") && IsEventStreamCollection(collectionName);
        static bool IsNonScopedEventStream(string collectionName)
            => !collectionName.Contains("x-") && IsEventStreamCollection(collectionName);
        static bool IsEventStreamCollection(string collectionName)
            => collectionName.Contains("stream-")
                && !collectionName.Contains("stream-definitions")
                && !collectionName.Contains("stream-processor-states");
        
        IEnumerable<string> GetSubscriptionStates()
            => _collectionNames.Where(_ => _.Contains("subscription-states"));
        IEnumerable<string> GetEventStreams()
            => _collectionNames.Where(_ => IsScopedEventStream(_) || IsNonScopedEventStream(_));
        IEnumerable<string> GetEventLogs()
            => _collectionNames.Where(_ => _.Contains("event-log"));
        IEnumerable<string> GetAggregates()
            => _collectionNames.Where(_ => _.Contains("aggregates"));
    }
}