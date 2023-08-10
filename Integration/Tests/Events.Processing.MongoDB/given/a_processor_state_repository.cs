// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using Events.Processing.MongoDB.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;

namespace Events.Processing.MongoDB.given;

public class a_processor_state_repository : IClassFixture<mongo_server_fixture>
{
    protected IStreamProcessorStateRepository repository { get; }

    public a_processor_state_repository(mongo_server_fixture fixture)
    {
        var nullLogger = NullLogger<a_processor_state_repository>.Instance;
        var collections = new StreamProcessorStateCollections(fixture.database_connection, nullLogger);
        repository = new StreamProcessorStateRepository(collections, nullLogger);
    }
}