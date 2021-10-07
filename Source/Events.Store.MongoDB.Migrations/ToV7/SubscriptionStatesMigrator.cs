// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="EventLogMigrator"/>.
    /// </summary>
    public class SubscriptionStatesMigrator : BaseMigrator<Old.Processing.Streams.EventHorizon.SubscriptionState, SubscriptionState>
    {
        public SubscriptionStatesMigrator(ICollectionNames collectionNames, IMongoCollectionMigrator migrator)
            : base(collectionNames, migrator)
        {
        }
        protected override IEnumerable<string> GetCollections(ICollectionNames collectionNames)
            => collectionNames.SubscriptionStates;
        protected override SubscriptionState Convert(Old.Processing.Streams.EventHorizon.SubscriptionState old)
            => new (
                old.Microservice,
                old.Tenant,
                old.Stream,
                old.Partition.ToString(),
                old.Position,
                old.RetryTime,
                old.FailureReason,
                old.ProcessingAttempts,
                old.LastSuccessfullyProcessed,
                old.IsFailing);
    }
}