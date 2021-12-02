// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dolittle.Runtime.Server;

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
        services.AddControllers();
        services.AddMvc();
        services.AddSwaggerGen();
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
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Runtime API v1");
        });

        app.UseStaticFiles();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}