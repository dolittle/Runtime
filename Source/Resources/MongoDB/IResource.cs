// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <summary>
    /// Defines a system that knows about the MongoDb resource.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the <see cref="MongoUrl"/> for the MongoDB resource.
        /// </summary>
        /// <returns>The <see cref="MongoUrl"/> connection string.</returns>
        MongoUrl GetConnectionString();
    }
}
