// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CLI.Options.Parsers.Versioning;
using Dolittle.Runtime.Versioning;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.CLI.Options.Parsers
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all implementations of <see cref="IValueParser"/> and dependencies to the provided service collection.
        /// </summary>
        /// <param name="services">The service collection to add value parsers in.</param>
        public static void AddValueParsers(this ServiceCollection services)
        {
            services.AddTransient<IVersionConverter, VersionConverter>();
            services.AddTransient<IValueParser, VersionParser>();
        }
    }
}