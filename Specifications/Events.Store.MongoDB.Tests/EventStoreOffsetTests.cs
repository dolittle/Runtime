// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Persistence;
using FluentAssertions;
using Xunit;

namespace Events.Store.MongoDB.Tests;

public class EventStoreOffsetTests(MongoDatabaseFixture fixture): IClassFixture<MongoDatabaseFixture>
{
    [Fact]
    public async Task WhenNoOffsetExists()
    {
        var nextOffset = await Sut.GetNextOffset(new ScopeId(Guid.NewGuid()), CancellationToken.None);

        nextOffset.Should().Be(0);
    }
    
    [Fact]
    public async Task WhenStoringDefaultScopeOffset()
    {
        ulong nextOffset = 9;
        await StoreOffset(ScopeId.Default, nextOffset);
        
        var nextOffsetRetrieved = await Sut.GetNextOffset(ScopeId.Default, CancellationToken.None);

        nextOffsetRetrieved.Should().Be(nextOffset);
    }
    
    [Fact]
    public async Task WhenUpdatingOffset()
    {
        var scope = new ScopeId(Guid.NewGuid());
        ulong nextOffset = 99;
        await StoreOffset(scope, nextOffset);
        await StoreOffset(scope, ++nextOffset);

        var nextOffsetRetrieved = await Sut.GetNextOffset(scope, CancellationToken.None);

        nextOffsetRetrieved.Should().Be(nextOffset);
    }
    
    [Fact]
    public async Task WhenRollingBackOffset()
    {
        var scope = new ScopeId(Guid.NewGuid());
        ulong nextOffset = 999;
        await StoreOffset(scope, nextOffset);
        await StoreOffsetButRollBack(scope, nextOffset + 1);

        var nextOffsetRetrieved = await Sut.GetNextOffset(scope, CancellationToken.None);

        nextOffsetRetrieved.Should().Be(nextOffset);
    }

    private async Task StoreOffset(ScopeId scope, ulong nextOffset)
    {
        using var session = await fixture.Connection.MongoClient.StartSessionAsync();
        session.StartTransaction();
        await Sut.UpdateOffset(session, scope, nextOffset, CancellationToken.None);
        await session.CommitTransactionAsync();
    }
    
    private async Task StoreOffsetButRollBack(ScopeId scope, ulong nextOffset)
    {
        using var session = await fixture.Connection.MongoClient.StartSessionAsync();
        session.StartTransaction();
        await Sut.UpdateOffset(session, scope, nextOffset, CancellationToken.None);
        await session.AbortTransactionAsync();
    }

    private EventLogOffsetStore Sut { get; } =  new(fixture.Connection);
}