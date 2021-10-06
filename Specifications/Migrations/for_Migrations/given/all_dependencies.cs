// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Migrations.for_Migrations.given
{
    public class all_dependencies
    {
        static ServiceCollection service_collection;
        Establish context = () =>
        {
            service_collection = new ServiceCollection();
        };

        protected static Migrations get_migrations() => new Migrations(service_collection.BuildServiceProvider());
        protected static void add_migrations(params ICanMigrateDataStores[] migrations)
        {
            foreach (var migration in migrations)    
            {
                service_collection.AddSingleton<ICanMigrateDataStores>(migration);
            }
        }
    }
}
