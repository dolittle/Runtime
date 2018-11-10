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
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Relativity.Protobuf;
using Dolittle.Runtime.Events.Relativity.Protobuf.Conversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Tenancy;
using Dolittle.Serialization.Protobuf;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity.Grpc
{
    /// <summary>
    /// Represents a concrete connection through a <see cref="IBarrier"/>
    /// </summary>
    public class QuantumTunnelConnection : IDisposable
    {
        readonly string _url;
        readonly IEnumerable<Dolittle.Artifacts.Artifact> _events;
        readonly ILogger _logger;
        readonly EventHorizonKey _horizonKey;
        readonly EventHorizonKey _destinationKey;
        readonly Channel _channel;
        readonly QuantumTunnelService.QuantumTunnelServiceClient _client;
        readonly FactoryFor<IGeodesics> _getGeodesics;
        readonly ISerializer _serializer;
        readonly FactoryFor<IEventStore> _getEventStore;
        readonly IScopedEventProcessingHub _eventProcessingHub;
        readonly CancellationTokenSource _runCancellationTokenSource;
        readonly CancellationToken _runCancellationToken;
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly ITenantOffsetRepository _tenantOffsetRepository;
        Thread _runThread = null;

        ParticleStreamProcessor _processor;
        


        /// <summary>
        /// Initializes a new instance of <see cref="QuantumTunnelConnection"/>
        /// </summary>
        /// <param name="horizonKey">The key for the connection</param>
        /// <param name="destinationKey">The key for the destination</param>
        /// <param name="url">Url for the <see cref="IEventHorizon"/> we're connecting to</param>
        /// <param name="events"><see cref="IEnumerable{Artifact}">Events</see> to connect for</param>
        /// <param name="getGeodesics">A <see cref="FactoryFor{IGeodesics}"/> to provide the correctly scoped geodesics instance for path offsetting</param>
        /// <param name="getEventStore">A factory to provide the correctly scoped <see cref="IEventStore"/> to persist incoming events to</param>
        /// <param name="eventProcessingHub"><see cref="IScopedEventProcessingHub"/> for processing incoming events</param>
        /// <param name="serializer"><see cref="ISerializer"/> to use for deserializing content of commits</param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> so we can set the correct context for the processing of the Events</param>
        /// <param name="tenants"><see cref="ITenants"/> the tenants that we need to be aware of from the other bounded contexts</param>
        /// <param name="tenantOffsetRepository"></param>
        public QuantumTunnelConnection(
                EventHorizonKey horizonKey,
                EventHorizonKey destinationKey,
                string url,
                IEnumerable<Dolittle.Artifacts.Artifact> events,
                FactoryFor<IGeodesics> getGeodesics,
                FactoryFor<IEventStore> getEventStore,
                IScopedEventProcessingHub eventProcessingHub,
                ISerializer serializer,
                ILogger logger,
                IExecutionContextManager executionContextManager,
                ITenants tenants,
                ITenantOffsetRepository tenantOffsetRepository)
        {
            _url = url;
            _events = events;
            _logger = logger;
            _horizonKey = horizonKey;
            _destinationKey = destinationKey;
            _getGeodesics = getGeodesics;
            _serializer = serializer;
            _getEventStore = getEventStore;
            _eventProcessingHub = eventProcessingHub;
            _channel = new Channel(_url, ChannelCredentials.Insecure);
            _client = new QuantumTunnelService.QuantumTunnelServiceClient(_channel);
            _runCancellationTokenSource = new CancellationTokenSource();
            _runCancellationToken = _runCancellationTokenSource.Token;
            _executionContextManager = executionContextManager;
            _processor = new ParticleStreamProcessor(getEventStore,getGeodesics,_horizonKey,eventProcessingHub,logger);
            _tenantOffsetRepository = tenantOffsetRepository;
            _tenants = tenants;

            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            AssemblyLoadContext.Default.Unloading += AssemblyLoadContextUnloading;
            Task.Run(() => Run(), _runCancellationToken);
            Console.CancelKeyPress += (s, e) => Close();
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
            _runCancellationTokenSource.Cancel();

            //if( _runThread != null ) _runThread.Abort();

            _logger.Information("Collapsing quantum tunnel");
            _channel.ShutdownAsync();
        }

        void Run()
        {
            _logger.Information($"Establishing connection towards event horizon for application ('{_destinationKey.Application}') and bounded context ('{_destinationKey.BoundedContext}') at '{_url}'");

            Task.Run(async() =>
            {
                _runThread = Thread.CurrentThread;
                for(;;)
                {
                    
                    _runCancellationToken.ThrowIfCancellationRequested();

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

        AsyncServerStreamingCall<Protobuf.CommittedEventStreamWithContext> GetOpenTunnel()
        {
            var tunnel = new OpenTunnel
            {
                Application = _horizonKey.Application.ToProtobuf(),
                BoundedContext = _horizonKey.BoundedContext.ToProtobuf(),
                ClientId = Guid.NewGuid().ToProtobuf()
            };
            tunnel.Offsets.AddRange(_tenantOffsetRepository.Get(_tenants.All, _horizonKey).Select(_ => _.ToProtobuf()));
            tunnel.Events.AddRange(_events.Select(_ => _.ToProtobuf()));
            return _client.Open(tunnel);
        }

        async Task OpenAndHandleStream()
        {
            _logger.Information($"Opening tunnel towards application '{_horizonKey.Application}' and bounded context '{_horizonKey.BoundedContext}'");

            var stream = GetOpenTunnel();
            try
            {
                while (await stream.ResponseStream.MoveNext(_runCancellationToken))
                {
                    _logger.Information("Commit received");
                    

                    _executionContextManager.CurrentFor(context.Tenant);
                    var seq = await _processor.Process(stream.ResponseStream.Current.Commit.ToCommittedEventStream());
                    _logger.Information($"Committed {seq}");
                }
            }
            catch (Exception moveException)
            {
                _logger.Error(moveException, "There was a problem moving to the next item in the stream");

            }

            _logger.Information("Done opening and handling the stream");
        }
    }
}