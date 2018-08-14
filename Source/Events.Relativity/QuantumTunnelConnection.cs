/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Logging;
using Dolittle.Serialization.Protobuf;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity
{

    /// <summary>
    /// Represents a concrete connection through a <see cref="IBarrier"/>
    /// </summary>
    public class QuantumTunnelConnection : IDisposable
    {
        readonly string _url;
        readonly IEnumerable<Artifact> _events;
        readonly ILogger _logger;
        readonly Application _application;
        readonly BoundedContext _boundedContext;
        readonly Channel _channel;
        readonly QuantumTunnelService.QuantumTunnelServiceClient _client;
        readonly Application _destinationApplication;
        readonly BoundedContext _destinationBoundedContext;
        readonly IGeodesics _geodesics;
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of <see cref="QuantumTunnelConnection"/>
        /// </summary>
        /// <param name="application">The current <see cref="Application"/></param>
        /// <param name="boundedContext">The current <see cref="BoundedContext"/></param>
        /// <param name="destinationApplication">The destination <see cref="Application"/></param>
        /// <param name="destinationBoundedContext">The destination <see cref="BoundedContext"/></param>
        /// <param name="url">Url for the <see cref="IEventHorizon"/> we're connecting to</param>
        /// <param name="events"><see cref="IEnumerable{Artifact}">Events</see> to connect for</param>
        /// <param name="geodesics"><see cref="IGeodesics"/> for path offsetting</param>
        /// <param name="serializer"><see cref="ISerializer"/> to use for deserializing content of commits</param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes</param>
        public QuantumTunnelConnection(
                Application application,
                BoundedContext boundedContext,
                Application destinationApplication,
                BoundedContext destinationBoundedContext,
                string url,
                IEnumerable<Artifact> events,
                IGeodesics geodesics,
                ISerializer serializer,
                ILogger logger)
            {
                _url = url;
                _events = events;
                _logger = logger;
                _application = application;
                _boundedContext = boundedContext;
                _destinationApplication = destinationApplication;
                _destinationBoundedContext = destinationBoundedContext;
                _channel = new Channel(_url, ChannelCredentials.Insecure);
                _client = new QuantumTunnelService.QuantumTunnelServiceClient(_channel);

                Task.Run(() => Run());
                _geodesics = geodesics;

                AppDomain.CurrentDomain.ProcessExit += ProcessExit;
                AssemblyLoadContext.Default.Unloading += AssemblyLoadContextUnloading;
                _serializer = serializer;
            }

            /// <summary>
            /// Destructs the <see cref="QuantumTunnelConnection"/>
            /// </summary>
            ~QuantumTunnelConnection()
            {
                Dispose();
            }

        /// <inheritdoc/>
        public void Dispose()
        {
            AppDomain.CurrentDomain.ProcessExit -= ProcessExit;
            AssemblyLoadContext.Default.Unloading -= AssemblyLoadContextUnloading;

            Close();
        }

        void ProcessExit(object sender, EventArgs e)
        {
            Close();
        }

        void AssemblyLoadContextUnloading(AssemblyLoadContext context)
        {
            Close();
        }

        void Close()
        {
            _logger.Information("Collapsing quantum tunnel");
            _channel.ShutdownAsync();
        }

        void Run()
        {
            _logger.Information($"Establishing connection towards event horizon for application ('{_destinationApplication}') and bounded context ('{_destinationBoundedContext}') at '{_url}'");

            Task.Run(async() =>
            {
                for (;;)
                {
                    try
                    {
                        await OpenAndHandleStream();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error occurred during establishing quantum tunnel");
                    }
                    _logger.Warning("Connection broken - backing off for a second");
                    Thread.Sleep(1000);
                    _logger.Warning("Trying to reconnect");
                }
            }).Wait();

            Close();
        }

        async Task OpenAndHandleStream()
        {
            _logger.Information($"Opening tunnel towards application '{_application}' and bounded context '{_boundedContext}'");

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

            var stream = _client.Open(openTunnelMessage);
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                _logger.Information("Commit received");

                try
                {
                    var current = stream.ResponseStream.Current.ToCommittedEventStream(_serializer);
                    _logger.Information($"CorrelationId : {current.CorrelationId}");
                    _logger.Information($"CommitId : {current.Id}");
                    _logger.Information($"EventSourceId : {current.Source.EventSource}");
                    _logger.Information($"EventSourceArtifact : {current.Source.Artifact}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Couldn't handle incoming commit");
                }
            }

            _logger.Information("Done opening and handling the stream");
        }

    }
}