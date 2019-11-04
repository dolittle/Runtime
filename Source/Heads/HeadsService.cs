/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;
using System.Timers;
using Dolittle.Collections;
using Dolittle.Heads.Runtime;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Time;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static Dolittle.Heads.Runtime.Heads;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents an implementation of <see cref="ClientBase"/>
    /// </summary>
    public class HeadsService : HeadsBase
    {
        readonly IConnectedHeads _connectedHeads;
        readonly ISystemClock _systemClock;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ILogger"/>
        /// </summary>
        /// <param name="connectedHeads"><see cref="IConnectedHeads"/> for working with connected heads</param>
        /// <param name="systemClock"><see cref="ISystemClock"/> for time</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public HeadsService(
            IConnectedHeads connectedHeads,
            ISystemClock systemClock,
            ILogger logger)
        {
            _connectedHeads = connectedHeads;
            _systemClock = systemClock;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void ClientDisconnected(Head client)
        {
            _connectedHeads.Disconnect(client.HeadId);
        }

        /// <inheritdoc/>
        public override Task Connect(HeadInfo request, IServerStreamWriter<Empty> responseStream, ServerCallContext context)
        {
            var headId = request.HeadId.To<HeadId>();
            try
            {
                _logger.Information($"Head connected '{headId}'");
                if (request.ServicesByName.Count == 0) _logger.Information("Not providing any head services");
                else request.ServicesByName.ForEach(_ => _logger.Information($"Providing service {_}"));

                var connectionTime = _systemClock.GetCurrentTime();
                var client = new Head(
                    headId,
                    request.Host,
                    request.Port,
                    request.Runtime,
                    request.ServicesByName,
                    connectionTime
                );

                _connectedHeads.Connect(client);

                var timer = new Timer(1000);
                timer.Enabled = true;
                timer.Elapsed += (s, e) => responseStream.WriteAsync(new Empty());

                context.CancellationToken.ThrowIfCancellationRequested();
                context.CancellationToken.WaitHandle.WaitOne();
            }
            finally
            {
                _connectedHeads.Disconnect(headId);
            }
            return Task.CompletedTask;
        }
    }
}