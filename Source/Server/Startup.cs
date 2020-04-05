// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// The startup for Asp.Net Core.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configure all services.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcWeb(_ => _.GrpcWebEnabled = true);

            services.AddCors(_ => _.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Grpc-Status", "Grpc-Message");
            }));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
        /// <param name="env"><see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
    }
}