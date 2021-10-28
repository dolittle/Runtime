// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CLI.Options.Parsers.Versioning;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.CLI.Options.Parsers.Aggregates;
using Dolittle.Runtime.CLI.Options.Parsers.Concepts;
using Dolittle.Runtime.CLI.Options.Parsers.EventHandlers;
using Dolittle.Runtime.CLI.Options.Parsers.Microservices;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
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
            services.AddTransient<IValueParser, MicroserviceAddressParser>();
            services.AddTransient<IVersionConverter, VersionConverter>();
            services.AddTransient<IValueParser, VersionParser>();
            services.AddTransient<IValueParser, GuidConceptParser<EventProcessorId>>();
            services.AddTransient<IValueParser, GuidConceptParser<ScopeId>>();
            services.AddTransient<IValueParser, GuidConceptParser<TenantId>>();
            services.AddTransient<IValueParser, UlongConceptParser<StreamPosition>>();
            services.AddTransient<IValueParser, EventHandlerIdOrAliasParser>();
            services.AddTransient<IValueParser, AggregateRootIdOrAliasParser>();
        }
    }
}
