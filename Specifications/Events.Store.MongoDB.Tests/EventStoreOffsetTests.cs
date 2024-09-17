// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Persistence;
using FluentAssertions;
using Xunit;

namespace Events.Store.MongoDB.Tests;

public class EventStoreOffsetTests(MongoDatabaseFixture fixture): IClassFixture<MongoDatabaseFixture>
{
    [Fact]
    public async Task WhenNoOffsetExists()
    {
        var nextOffset = await Sut.GetNextOffset(Guid.NewGuid().ToString(), null, CancellationToken.None);

        nextOffset.Should().Be(0);
    }
    
    [Fact]
    public async Task WhenStoringDefaultScopeOffset()
    {
        ulong nextOffset = 9;
        await StoreOffset("event-log", nextOffset);
        
        var nextOffsetRetrieved = await Sut.GetNextOffset("event-log", null, CancellationToken.None);

        nextOffsetRetrieved.Should().Be(nextOffset);
    }
    
    [Fact]
    public async Task WhenUpdatingOffset()
    {
        var stream = Guid.NewGuid().ToString();
        ulong nextOffset = 99;
        await StoreOffset(stream, nextOffset);
        await StoreOffset(stream, ++nextOffset);

        var nextOffsetRetrieved = await Sut.GetNextOffset(stream, null, CancellationToken.None);

        nextOffsetRetrieved.Should().Be(nextOffset);
    }
    
    [Fact]
    public async Task WhenRollingBackOffset()
    {
        var stream = Guid.NewGuid().ToString("N");
        ulong nextOffset = 999;
        await StoreOffset(stream, nextOffset);
        await StoreOffsetButRollBack(stream, nextOffset + 1);

        var nextOffsetRetrieved = await Sut.GetNextOffset(stream, null, CancellationToken.None);

        nextOffsetRetrieved.Should().Be(nextOffset);
    }

    private async Task StoreOffset(string stream, ulong nextOffset)
    {
        using var session = await fixture.Connection.MongoClient.StartSessionAsync();
        session.StartTransaction();
        await Sut.UpdateOffset(stream, session, nextOffset, CancellationToken.None);
        await session.CommitTransactionAsync();
    }
    
    private async Task StoreOffsetButRollBack(string stream, ulong nextOffset)
    {
        using var session = await fixture.Connection.MongoClient.StartSessionAsync();
        session.StartTransaction();
        await Sut.UpdateOffset(stream, session, nextOffset, CancellationToken.None);
        await session.AbortTransactionAsync();
    }

    private OffsetStore Sut { get; } =  new(fixture.Connection);
}