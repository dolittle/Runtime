// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Server.HealthChecks
{
    /// <summary>
    /// Represents an implementation of <see cref="IConfigureOptions{TOptions}"/> for <see cref="HealthCheckOptions"/> that configure the health check endpoint response.
    /// </summary>
    public class EndpointConfiguration : IConfigureOptions<HealthCheckOptions>
    {
        /// <inheritdoc />
        public void Configure(HealthCheckOptions options)
        {
            options.ResponseWriter = WriteReportResponse;
        }

        static Task WriteReportResponse(HttpContext context, HealthReport report)
            => context.Response.WriteAsJsonAsync(report, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            });
    }
}