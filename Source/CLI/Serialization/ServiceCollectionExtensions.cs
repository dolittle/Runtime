// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.CLI.Serialization
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the services related to serialization to the provided service collection.
        /// </summary>
        /// <param name="services">The service collection to add serialization services to.</param>
        public static void AddSerializers(this ServiceCollection services)
        {
            var converters = new StaticConverterProvider(
                new ConceptConverter());
            var providers = new StaticConverterProviders(converters);
            var serializer = new Serializer(providers);
            services.AddSingleton<ISerializer>(serializer);
        }
    }
}
