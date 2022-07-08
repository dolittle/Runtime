// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;

namespace Dolittle.Runtime.DependencyInversion.Logging;

/// <summary>
/// Extension methods for <see cref="ContainerBuilder"/>.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Adds handling of <see cref="Microsoft.Extensions.Logging"/> injection to the container.
    /// </summary>
    /// <param name="builder">The container builder to add resolving middleware to.</param>
    /// <returns>The container builder for continuation</returns>
    public static ContainerBuilder AddLogging(this ContainerBuilder builder)
    {
        builder.RegisterModule<LoggerModule>();
        return builder;
    }
}
