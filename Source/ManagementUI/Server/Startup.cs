// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;
using Dolittle.Booting;
using Dolittle.DependencyInversion.Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Server
{
    /// <summary>
    /// The ASP.NET Core Startup class.
    /// </summary>
    public class Startup
    {
        BootloaderResult _bootResult;
        ILoggerFactory _loggerFactory;

        /// <summary>
        /// Configure services.
        /// </summary>
        /// <param name="services">Service collection to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddEventSourceLogger();
            });

            services.AddMvc();

            _bootResult = services.AddDolittle(_loggerFactory);
        }

        /// <summary>
        /// Configure the Autofac IoC Container.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddDolittle(_bootResult.Assemblies, _bootResult.Bindings);
        }

        /// <summary>
        /// Configure app.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDolittle();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(_ => _.MapControllers());

            app.RunAsSinglePageApplication();
        }
    }
}
