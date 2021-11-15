// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Resources.Contracts;

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <summary>
    /// Defines a system that knows how to get the MongoDB resource.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Gets the MongoDB resource for a specific tenant.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns>The <see cref="GetMongoDbResponse"/> response.</returns>
        GetMongoDBResponse GetResource(ExecutionContext executionContext);
    }
}
