// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters
{
    public class SubscriptionStatesConverter : IConvertFromOldToNew<Old.Processing.Streams.EventHorizon.SubscriptionState, SubscriptionState>
    {
        public FilterDefinition<Old.Processing.Streams.EventHorizon.SubscriptionState> Filter { get; } = Builders<Old.Processing.Streams.EventHorizon.SubscriptionState>.Filter.Empty;
        public SubscriptionState Convert(Old.Processing.Streams.EventHorizon.SubscriptionState old)
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