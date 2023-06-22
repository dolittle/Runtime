// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using MongoDB.Driver;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;
using StreamEvent = Dolittle.Runtime.Events.Store.MongoDB.Events.StreamEvent;

namespace Integration.Tests.Events.Store.when_writing_to_stream.that_is_not_scoped;

class when_writing_to_stream : given.a_clean_event_store
{
    static readonly FilterDefinition<StreamEvent> all_filter = Builders<StreamEvent>.Filter.Empty;
    static readonly ScopeId scope = ScopeId.Default;
    static UncommittedEvents uncommitted_events_list;
    static CommittedEvents committed_events;
    static StreamId stream;
    static PartitionId partition;
    static Exception failure;
    static IMongoCollection<StreamEvent> mongo_stream;

    Establish context = () =>
    {
        stream = Guid.Parse("a4b11a07-1939-4058-92ae-78735d686d77");
        partition = "some partition";
        uncommitted_events_list = new UncommittedEvents(new List<UncommittedEvent>
        {
            given.event_to_commit.create(),
            given.event_to_commit.create(),
            given.event_to_commit.create(),
            given.event_to_commit.create(),
        });
        var response = event_store.Commit(uncommitted_events_list, execution_context).GetAwaiter().GetResult();
        mongo_stream = streams.Get(scope, stream, CancellationToken.None).GetAwaiter().GetResult();
        committed_events = response.Events.ToCommittedEvents();
    };
    
    
    [Tags("IntegrationTest")]
    class and_stream_is_empty
    {
        Because of = () => failure = Catch.Exception(() => events_to_streams_writer.Write(committed_events.Select(_ => (_, partition with {Value = "partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult());
        
        It should_not_fail = () => failure.ShouldBeNull();
        It should_have_4_events_in_the_stream = () => mongo_stream.CountDocuments(all_filter).ShouldEqual(4);
        
    }
    
    [Tags("IntegrationTest")]
    class and_writing_non_duplicate_events
    {
        Establish context = () => events_to_streams_writer.Write(committed_events.Take(2).Select(_ => (_, partition with {Value = "partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult();
        
        Because of = () => failure = Catch.Exception(() => events_to_streams_writer.Write(committed_events.Skip(2).Select(_ => (_, partition with {Value = "partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult());

        It should_not_fail = () => failure.ShouldBeNull();
        It should_have_4_events_in_the_stream = () => mongo_stream.CountDocuments(all_filter).ShouldEqual(4);
    }
    
    [Tags("IntegrationTest")]
    class and_writing_similar_duplicate_events
    {
        Establish context = () => events_to_streams_writer.Write(committed_events.Take(3).Select(_ => (_, partition with {Value = "partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult();
        
        Because of = () => failure = Catch.Exception(() => events_to_streams_writer.Write(committed_events.Select(_ => (_, partition with {Value = "partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult());

        It should_not_fail = () => failure.ShouldBeNull();
        It should_have_4_events_in_the_stream = () => mongo_stream.CountDocuments(all_filter).ShouldEqual(4);
    }
    
    [Tags("IntegrationTest")]
    class and_writing_non_similar_duplicate_events
    {
        Establish context = () => events_to_streams_writer.Write(committed_events.Take(3).Select(_ => (_, partition with {Value = "partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult();
        
        Because of = () => failure = Catch.Exception(() => events_to_streams_writer.Write(committed_events.Select(_ => (_, partition with {Value = "another partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult());

        It should_fail = () => failure.ShouldNotBeNull();
        It should_have_only_3_events_in_the_stream = () => mongo_stream.CountDocuments(all_filter).ShouldEqual(3);
    }
    
    [Tags("IntegrationTest")]
    class and_writing_an_event_twice
    {
        Establish context = () => events_to_streams_writer.Write(committed_events.Select(_ => (_, partition with {Value = "partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult();
        
        Because of = () => failure = Catch.Exception(() => events_to_streams_writer.Write(committed_events.Skip(3).Select(_ => (_, partition with {Value = "partition"})), scope, stream, CancellationToken.None).GetAwaiter().GetResult());

        It should_not_fail = () => failure.ShouldBeNull();
        It should_have_4_events_in_the_stream = () => mongo_stream.CountDocuments(all_filter).ShouldEqual(4);
    }
}