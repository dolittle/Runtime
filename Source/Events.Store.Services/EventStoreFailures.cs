// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Services;

/// <summary>
/// Holds the unique <see cref="FailureId"> failure ids </see> unique to the Event Store.
/// </summary>
public static class EventStoreFailures
{
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventStoreUnavailable' failure type.
    /// </summary>
    public static readonly FailureId EventStoreUnavailable = FailureId.Create("b6fcb5dd-a32b-435b-8bf4-ed96e846d460");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventAppliedByOtherAggregateRoot' failure type.
    /// </summary>
    public static readonly FailureId EventAppliedByOtherAggregateRoot = FailureId.Create("d08a30b0-56ab-43dc-8fe6-490320514d2f");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventAppliedToOtherEventSource' failure type.
    /// </summary>
    public static readonly FailureId EventAppliedToOtherEventSource = FailureId.Create("b2acc526-ba3a-490e-9f15-9453c6f13b46");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventStorePersistanceError' failure type.
    /// </summary>
    public static readonly FailureId EventStorePersistanceError = FailureId.Create("ad55fca7-476a-4f68-9411-1a3b087ab843");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventStoreConsistencyError' failure type.
    /// </summary>
    public static readonly FailureId EventStoreConsistencyError = FailureId.Create("6f0e6cab-c7e5-402e-a502-e095f9545297");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventLogSequenceIsOutOfOrder' failure type.
    /// </summary>
    public static readonly FailureId EventLogSequenceIsOutOfOrder = FailureId.Create("eb508238-87ff-4519-a743-03be5196a83d");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'EventCannotBeNull' failure type.
    /// </summary>
    public static readonly FailureId EventCannotBeNull = FailureId.Create("45a811d9-bdf7-4ee1-b9bc-3f248e761799");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'AggregateRootVersionOutOfOrder' failure type.
    /// </summary>
    public static readonly FailureId AggregateRootVersionOutOfOrder = FailureId.Create("eb51284e-c7b4-4966-8da4-64a862f07560");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'AggregateRootConcurrencyConflict' failure type.
    /// </summary>
    public static readonly FailureId AggregateRootConcurrencyConflict = FailureId.Create("f25cccfb-3ae1-4969-bee6-906370ffbc2d");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'NoEventsToCommit' failure type.
    /// </summary>
    public static readonly FailureId NoEventsToCommit = FailureId.Create("ef3f1a42-9bc3-4d98-aa2a-942db7c56ac1");
}