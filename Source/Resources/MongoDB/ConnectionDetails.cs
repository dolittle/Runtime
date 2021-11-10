// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <summary>
    /// Represents the connection details for a MongoDB resource.
    /// </summary>
    /// <param name="ConnectionString">The connection string.</param>
    /// <param name="DatabaseName">The MongoDB resource database name.</param>
    public record ConnectionDetails(MongoUrl ConnectionString, string DatabaseName);
}
