/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Logging;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents a concrete connection through a <see cref="IBarrier"/>
    /// </summary>
    public class Connection
    {
        readonly string _url;
        readonly IEnumerable<Artifact> _events;
        readonly ILogger _logger;
        readonly Application _application;
        readonly BoundedContext _boundedContext;

        /// <summary>
        /// Initializes a new instance of <see cref="Connection"/>
        /// </summary>
        /// <param name="application">The current <see cref="Application"/></param>
        /// <param name="boundedContext">The current <see cref="BoundedContext"/></param>
        /// <param name="url">Url for the <see cref="IEventHorizon"/> we're connecting to</param>
        /// <param name="events"><see cref="IEnumerable{Artifact}">Events</see> to connect for</param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes</param>
        public Connection(Application application, BoundedContext boundedContext, string url, IEnumerable<Artifact> events, ILogger logger)
        {
            _url = url;
            _events = events;
            _logger = logger;
            _application = application;
            _boundedContext = boundedContext;

            Run();
        }

        void Run()
        {
            _logger.Information($"Establishing connection towards event horizon at '{_url}'");
            var channel = new Channel(_url, ChannelCredentials.Insecure);
            var client = new QuantumTunnelService.QuantumTunnelServiceClient(channel);

            Task.Run(async() =>
            {
                for (;;)
                {
                    try
                    {
                        await OpenAndHandleStream(client);
                    }
                    catch
                    {

                    }
                    _logger.Warning("Connection broken - backing off for a second");
                    Thread.Sleep(1000);
                    _logger.Warning("Trying to reconnect");
                }
            }).Wait();

            channel.ShutdownAsync();
        }

        async Task OpenAndHandleStream(QuantumTunnelService.QuantumTunnelServiceClient client)
        {
            var openTunnelMessage = new OpenTunnelMessage
            {
                Application = ByteString.CopyFrom(_application.Value.ToByteArray()),
                BoundedContext = ByteString.CopyFrom(_boundedContext.Value.ToByteArray()),
            };

            _events.Select(_ => new EventArtifactMessage
            {
                Event = ByteString.CopyFrom(_.Id.Value.ToByteArray()),
                Generation = _.Generation
            }).ForEach(openTunnelMessage.Events.Add);

            var stream = client.Open(openTunnelMessage);
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                _logger.Information("Event received");
            }
        }
    }
}