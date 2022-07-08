// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Dolittle.Runtime.Resources.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IKnowTheConnectionString"/>.
/// </summary>
[Singleton, PerTenant] // TODO: Should things like this really be a singleton?
public class ConnectionStringFromResourceConfiguration : IKnowTheConnectionString
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringFromResourceConfiguration"/> class. 
    /// </summary>
    /// <param name="configuration">The <see cref="IOptions{T}"/> of type <see cref="MongoDBConfiguration"/> to use.</param>
    public ConnectionStringFromResourceConfiguration(IOptions<MongoDBConfiguration> configuration)
    {
        ConnectionString = BuildConnectionString(configuration.Value);
    }

    /// <inheritdoc />
    public MongoUrl ConnectionString { get; }

    static MongoUrl BuildConnectionString(MongoDBConfiguration configuration)
        => new MongoUrlBuilder(configuration.Host)
        {
            UseTls = configuration.UseSSL,
            DatabaseName = configuration.Database
        }.ToMongoUrl();
}
