// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents a simple middleware that proxies requests to a relative path to a defined target.
    /// </summary>
    public class ProxyServerMiddleware
    {
        readonly string _route;
        readonly string _target;
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of <see cref="ProxyServerMiddleware"/>.
        /// </summary>
        /// <param name="route">Route to represent.</param>
        /// <param name="route">The relative - rooted route (e.g. /proxy).</param>
        /// <param name="target">The fully qualified URL to proxy through to.</param>
        /// <param name="next">Next <see cref="RequestDelegate">middleware</see>.</param>
        public ProxyServerMiddleware(string route, string target, RequestDelegate next)
        {
            _route = route;
            _target = target;
            _next = next;
        }

        /// <summary>
        /// Middleware invoke handler.
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/> to work with.</param>
        /// <returns>Asynchronous <see cref="Task"/>.</returns>
        public async Task Invoke(HttpContext context)
        {
            var endRequest = false;
            if (context.Request.Path.Value.Equals(_route, StringComparison.OrdinalIgnoreCase))
            {
                await StreamAsync(context, _target).ConfigureAwait(false);
                endRequest = true;
            }
            if (!endRequest)
            {
                await _next(context).ConfigureAwait(false);
            }
        }

        async Task StreamAsync(HttpContext context, string url)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            var webRequest = new HttpClient(httpClientHandler);

            var buffer = new byte[4 * 1024];
            var localResponse = context.Response;
            try
            {
                using var remoteStream = await webRequest.GetStreamAsync(url).ConfigureAwait(false);
                var bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                localResponse.Clear();
                localResponse.ContentType = "application/text";

                while (bytesRead > 0)
                {
                    await localResponse.Body.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                    bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
