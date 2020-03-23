// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using System.Timers;
using contracts::Dolittle.Runtime.Heads;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Time;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Heads.Heads;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents an implementation of <see cref="ClientBase"/>.
    /// </summary>
    public class HeadsService : HeadsBase
    {
        readonly IConnectedHeads _connectedHeads;
        readonly ISystemClock _systemClock;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadsService"/> class.
        /// </summary>
        /// <param name="connectedHeads"><see cref="IConnectedHeads"/> for working with connected heads.</param>
        /// <param name="systemClock"><see cref="ISystemClock"/> for time.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public HeadsService(
            IConnectedHeads connectedHeads,
            ISystemClock systemClock,
            ILogger logger)
        {
            _connectedHeads = connectedHeads;
            _systemClock = systemClock;
            _logger = logger;
        }

        /// <summary>
        /// Signals a <see cref="Head"/> client has disconnected.
        /// </summary>
        /// <param name="client"><see cref="Head"/> to disconnect.</param>
        public void ClientDisconnected(Head client)
        {
            _connectedHeads.Disconnect(client.HeadId);
        }

        /// <inheritdoc/>
        public override Task Connect(HeadInfo request, IServerStreamWriter<Empty> responseStream, ServerCallContext context)
        {
            var headId = request.HeadId.To<HeadId>();
            Timer timer = null;
            try
            {
                _logger.Information($"Head connected '{headId}'");

                var connectionTime = _systemClock.GetCurrentTime();
                var client = new Head(
                    headId,
                    request.Host,
                    request.Runtime,
                    request.Version,
                    connectionTime);

                _connectedHeads.Connect(client);

                timer = new Timer(1000)
                {
                    Enabled = true
                };
                timer.Elapsed += (s, e) => responseStream.WriteAsync(new Empty());

                context.CancellationToken.ThrowIfCancellationRequested();
                context.CancellationToken.WaitHandle.WaitOne();
            }
            finally
            {
                _connectedHeads.Disconnect(headId);
                timer?.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}