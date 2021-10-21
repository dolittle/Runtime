// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using McMaster.Extensions.CommandLineUtils.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.CLI.Options.Parsers
{
    public static class ValueParserProviderExtensions
    {
        /// <summary>
        /// Registers all instances of <see cref="IValueParser"/> in the value parser provider.
        /// </summary>
        /// <param name="provider">The value parser provider to register parsers in.</param>
        /// <param name="container">The service provider to get value parsers from.</param>
        public static void UseAllValueParsers(this ValueParserProvider provider, ServiceProvider container)
        {
            foreach (var parser in container.GetServices<IValueParser>())
            {
                provider.Add(parser);
            }
        }
    }
}
