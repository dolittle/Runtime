// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Autofac;
using Dolittle.Booting;
using Dolittle.DependencyInversion.Autofac;
using Dolittle.DependencyInversion.Strategies;
using Dolittle.Execution;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// The startup for Asp.Net Core.
    /// </summary>
    public class Startup : IDisposable
    {
        readonly IWebHostEnvironment _env;
        ILoggerFactory _loggerFactory;
        BootloaderResult _bootResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env"><see cref="IWebHostEnvironment" />.</param>
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// Configure all services.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddEventSourceLogger();
            });

            services.AddGrpc();
            services.AddGrpcWeb(_ => _.GrpcWebEnabled = true);

            services.AddCors(_ => _.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Grpc-Status", "Grpc-Message");
            }));

            _bootResult = services.AddDolittle(
                builder =>
                {
                    if (_env.IsDevelopment()) builder.Development();
                },
                _loggerFactory);
            ManagementLogAppender.LoggerFactory = _loggerFactory;
            var loggingBootStageResult = _bootResult.BootStageResults.ToArray()[1];
            ManagementLogAppender.ExecutionContextManager = ((Constant)loggingBootStageResult.Bindings.First(_ => _.Service == typeof(IExecutionContextManager)).Strategy).Target as IExecutionContextManager;
        }

        /// <summary>
        /// Configure the Autofac container.
        /// </summary>
        /// <param name="containerBuilder"><see cref="ContainerBuilder"/> to configure.</param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddDolittle(_bootResult.Assemblies, _bootResult.Bindings);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
        /// <param name="env"><see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDolittle();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseStaticFiles();

            app.UseGrpcWeb();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<Tenancy.Management.TenantsService>().EnableGrpcWeb().RequireCors("AllowAll");
                endpoints.MapGrpcService<Heads.Management.HeadsService>().EnableGrpcWeb().RequireCors("AllowAll");
                endpoints.MapGrpcService<Logging.Management.LogService>().EnableGrpcWeb().RequireCors("AllowAll");
            });

            app.RunAsSinglePageApplication();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _loggerFactory.Dispose();
        }
    }
}