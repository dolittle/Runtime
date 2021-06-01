// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Management
{
    /// <summary>
    /// Represents extension methods for working with exposing the GraphQL endpoints for management.
    /// </summary>
    public static class GraphQLEndpoints
    {
        /// <summary>
        /// Adds the GraphQL API for management to the HotChocolate GraphQL builder.
        /// </summary>
        /// <param name="graphQLBuilder"><see cref="IRequestExecutorBuilder"/> to add to.</param>
        /// <returns><see cref="IRequestExecutorBuilder"/> for continuation.</returns>
        public static IRequestExecutorBuilder AddManagementAPI(this IRequestExecutorBuilder graphQLBuilder)
        {
            graphQLBuilder.AddQueryType<Query>();
            return graphQLBuilder;
        }
    }
}