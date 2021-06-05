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
    public class ProxyServerMiddleware
    {
        readonly string _route;
        readonly string _target;
        private readonly RequestDelegate _next;

        public ProxyServerMiddleware(string route, string target, RequestDelegate next)
        {
            _route = route;
            _target = target;
            _next = next;
        }

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

        private static async Task StreamAsync(HttpContext context, string url)
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

                while (bytesRead > 0) // && localResponse.IsClientConnected)
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
