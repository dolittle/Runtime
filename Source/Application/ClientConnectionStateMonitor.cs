/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static Dolittle.Runtime.Application.Grpc.Client.ConnectionStatus;

namespace Dolittle.Runtime.Application
{

    /// <summary>
    /// Represents an implementation of <see cref="IClientConnectionStateMonitor"/>
    /// </summary>
    [Singleton]
    public class ClientConnectionStateMonitor : IClientConnectionStateMonitor
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ClientConnectionStateMonitor"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public ClientConnectionStateMonitor(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Monitor(Client client)
        {
            _logger.Information($"Monitor client '{client.ClientId}'");

            Task.Run(async() =>
            {
                var ctx = new CancellationTokenSource();
                ctx.Token.ThrowIfCancellationRequested();

                AsyncDuplexStreamingCall<Empty, Empty> streams = null;

                var started = false;
                var clientConnected = false;
                var lastPing = DateTime.UtcNow;

                var timer = new System.Timers.Timer(1000);
                timer.AutoReset = true;
                timer.Enabled = true;

                void Shutdown()
                {
                    if (!started) return;
                    _logger.Information($"Client '{client.ClientId}' shutdown");

                    started = false;
                    ctx.Cancel();
                    timer.Stop();
                    client.OnDisconnected();
                }

                timer.Elapsed += (s, e) =>
                {
                    if (ctx.Token.IsCancellationRequested)
                    {
                        Shutdown();
                        return;
                    }
                    if (started)
                    {
                        if (clientConnected)
                        {
                            var now = DateTime.UtcNow;
                            var delta = now.Subtract(lastPing);

                            if (now.Subtract(lastPing).TotalMilliseconds > 1000) Shutdown();
                        }
                        if (client.Channel is Channel channel)
                        {
                            var state = channel.State;
                            if (state == ChannelState.Idle ||
                                state == ChannelState.Shutdown) Shutdown();
                        }
                    }
                    streams?.RequestStream.WriteAsync(new Empty());
                };

                var connectionStatusClient = new ConnectionStatusClient(client.Channel);
                _logger.Information($"Connect to connection status service on client ({client.Host}:{client.Port})");
                streams = connectionStatusClient.Connect(cancellationToken: ctx.Token);
                timer.Start();

                _logger.Information("Wait for client");
                started = true;

                try
                {
                    while (await streams.ResponseStream.MoveNext(ctx.Token))
                    {
                        clientConnected = true;
                        lastPing = DateTime.UtcNow;
                    }
                }
                catch { }

                Shutdown();
            });
        }
    }
}