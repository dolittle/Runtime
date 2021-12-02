// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Resources.Contracts;

namespace Dolittle.Runtime.Resources.MongoDB;

/// <summary>
/// Defines a system that can get the MongoDB resource details for a specific tenant.
/// </summary>
public interface ICanGetResourceForTenant
{
    /// <summary>
    /// Gets the MongoDB resource for a specific tenant.
    /// </summary>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> that specifies the tenant..</param>
    /// <returns>The <see cref="GetMongoDBResponse"/> with the details for using the MongoDB resource.</returns>
    GetMongoDBResponse GetResource(ExecutionContext executionContext);
}