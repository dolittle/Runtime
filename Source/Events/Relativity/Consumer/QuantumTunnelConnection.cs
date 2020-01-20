// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Events.Relativity.Microservice;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Relativity.Protobuf.Conversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Tenancy;
using Dolittle.Serialization.Protobuf;
using Grpc.Core;
using grpc = Dolittle.Events.Relativity.Microservice;

namespace Dolittle.Runtime.Events.Relativity.Grpc
{
    /// <summary>
    /// Represents a concrete connection through a <see cref="IBarrier"/>.
    /// </summary>
    public class QuantumTunnelConnection : IDisposable
    {
        readonly string _address;
        readonly IEnumerable<Dolittle.Artifacts.Artifact> _events;
        readonly ILogger _logger;
        readonly EventHorizonKey _horizonKey;
        readonly EventHorizonKey _destinationKey;
        readonly Channel _channel;
        readonly QuantumTunnelService.QuantumTunnelServiceClient _client;
        readonly FactoryFor<IGeodesics> _getGeodesics;
        readonly ISerializer _serializer;
        readonly FactoryFor<IEventStore> _getEventStore;
        readonly IStreamProcessingHub _eventProcessingHub;
        readonly CancellationTokenSource _runCancellationTokenSource;
        readonly CancellationToken _runCancellationToken;
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly ITenantOffsetRepository _tenantOffsetRepository;
        readonly ParticleStreamProcessor _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuantumTunnelConnection"/> class.
        /// </summary>
        /// <param name="horizonKey">The key for the connection.</param>
        /// <param name="destinationKey">The key for the destination.</param>
        /// <param name="address">Url for the <see cref="IEventHorizon"/> we're connecting to.</param>
        /// <param name="events"><see cref="IEnumerable{Artifact}">Events</see> to connect for.</param>
        /// <param name="getGeodesics">A <see cref="FactoryFor{IGeodesics}"/> to provide the correctly scoped geodesics instance for path offsetting.</param>
        /// <param name="getEventStore">A factory to provide the correctly scoped <see cref="IEventStore"/> to persist incoming events to.</param>
        /// <param name="eventProcessingHub"><see cref="IStreamProcessingHub"/> for processing incoming events.</param>
        /// <param name="serializer"><see cref="ISerializer"/> to use for deserializing content of commits.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> so we can set the correct context for the processing of the Events.</param>
        /// <param name="tenants"><see cref="ITenants"/> the tenants that we need to be aware of from the other bounded contexts.</param>
        /// <param name="tenantOffsetRepository"><see creF="ITenantOffsetRepository"/> to use for tracking offsets per tenant.</param>
        public QuantumTunnelConnection(
            EventHorizonKey horizonKey,
            EventHorizonKey destinationKey,
            string address,
            IEnumerable<Dolittle.Artifacts.Artifact> events,
            FactoryFor<IGeodesics> getGeodesics,
            FactoryFor<IEventStore> getEventStore,
            IStreamProcessingHub eventProcessingHub,
            ISerializer serializer,
            ILogger logger,
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            ITenantOffsetRepository tenantOffsetRepository)
        {
            _address = address;
            _events = events;
            _logger = logger;
            _horizonKey = horizonKey;
            _destinationKey = destinationKey;
            _getGeodesics = getGeodesics;
            _serializer = serializer;
            _getEventStore = getEventStore;
            _eventProcessingHub = eventProcessingHub;
            _channel = new Channel(_address, ChannelCredentials.Insecure);
            _client = new QuantumTunnelService.QuantumTunnelServiceClient(_channel);
            _runCancellationTokenSource = new CancellationTokenSource();
            _runCancellationToken = _runCancellationTokenSource.Token;
            _executionContextManager = executionContextManager;
            _processor = new ParticleStreamProcessor(getEventStore, getGeodesics, _destinationKey, eventProcessingHub, _executionContextManager, logger);
            _tenantOffsetRepository = tenantOffsetRepository;
            _tenants = tenants;

            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            AssemblyLoadContext.Default.Unloading += AssemblyLoadContextUnloading;
            Task.Run(() => Run(), _runCancellationToken);
            Console.CancelKeyPress += (s, e) => Close();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="QuantumTunnelConnection"/> class.
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
            _runCancellationTokenSource.Dispose();

            _logger.Information("Collapsing quantum tunnel");
            _channel.ShutdownAsync();
        }

        void Run()
        {
            _logger.Information($"Establishing connection towards event horizon for application ('{_destinationKey.Application}') and bounded context ('{_destinationKey.BoundedContext}') at '{_address}'");

            Task.Run(async () =>
           {
               while (true)
               {
                   _runCancellationToken.ThrowIfCancellationRequested();

                   try
                   {
                       await OpenAndHandleStream().ConfigureAwait(false);
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

        AsyncServerStreamingCall<grpc.CommittedEventStreamWithContext> GetOpenTunnel()
        {
            var tunnel = new OpenTunnel
            {
                Application = _destinationKey.Application.ToProtobuf(),
                BoundedContext = _destinationKey.BoundedContext.ToProtobuf(),
                ClientId = Guid.NewGuid().ToProtobuf()
            };
            tunnel.Offsets.AddRange(_tenantOffsetRepository.Get(_tenants.All, _destinationKey).Select(_ => _.ToProtobuf()));
            tunnel.Events.AddRange(_events.Select(_ => _.ToProtobuf()));
            return _client.Open(tunnel);
        }

        async Task OpenAndHandleStream()
        {
            _logger.Information($"Opening tunnel towards application '{_destinationKey.Application}' and bounded context '{_destinationKey.BoundedContext}'");

            var stream = GetOpenTunnel();
            try
            {
                while (await stream.ResponseStream.MoveNext(_runCancellationToken).ConfigureAwait(false))
                {
                    _logger.Information("Commit received");

                    var seq = await _processor.Process(stream.ResponseStream.Current.ToCommittedEventStreamWithContext()).ConfigureAwait(false);
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
