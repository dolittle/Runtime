// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <summary>
    /// Defines a system that knows about the MongoDb resource.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the <see cref="ConnectionDetails"/> for the MongoDB resource.
        /// </summary>
        /// <returns></returns>
        ConnectionDetails GetConnectionDetails();
    }
}
