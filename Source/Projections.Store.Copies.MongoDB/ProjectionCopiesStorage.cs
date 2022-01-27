// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Resources.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopiesStorage"/>.
/// </summary>
public class ProjectionCopiesStorage : IProjectionCopiesStorage
{
    public ProjectionCopiesStorage(IKnowTheConnectionString readModelsDatabase)
    {
        var connectionString = readModelsDatabase.ConnectionString;

        var settings = MongoClientSettings.FromUrl(connectionString);
        settings.GuidRepresentation = GuidRepresentation.Standard;
        
        var client = new MongoClient(settings.Freeze());
        Database = client.GetDatabase(connectionString.DatabaseName);
    }

    /// <inheritdoc />
    public IMongoDatabase Database { get; }
}
