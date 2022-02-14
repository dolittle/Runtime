// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services related to management of Projections.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddProjectionServices(this IServiceCollection services)
    {
        services.AddTransient<IManagementClient, ManagementClient>();
        services.AddTransient<IConvertProjectionDefinitions, ConvertProjectionDefinitions>();
        services.AddTransient<IResolveProjectionIdAndScope, ProjectionIdAndScopeResolver>();
    }
}
