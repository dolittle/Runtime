// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Logging.Management;
using Dolittle.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Logging.Management.Log;

namespace Dolittle.Runtime.Logging.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="LogBase"/>.
    /// </summary>
    public class LogService : LogBase
    {
        readonly ILogManager _logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogService"/> class.
        /// </summary>
        /// <param name="logManager">The <see cref="ILogManager"/>.</param>
        public LogService(ILogManager logManager)
        {
            _logManager = logManager;
        }

        /// <inheritdoc/>
        public override async Task Listen(Empty request, IServerStreamWriter<LogMessage> responseStream, ServerCallContext context)
        {
            var queue = new Queue<LogMessage>();
            using var autoResetEvent = new AutoResetEvent(false);

            void StateChanged(object sender, NotifyCollectionChangedEventArgs args)
            {
                if (args.NewItems != default)
                {
                    var items = new List<string>();
                    foreach (var item in args.NewItems)
                    {
                        items.Add(item as string);
                    }

                    GetLogMessagesFrom(items).ForEach(queue.Enqueue);
                }
            }

            try
            {
                _logManager.Messages.CollectionChanged += StateChanged;

                context.CancellationToken.ThrowIfCancellationRequested();

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    while (queue.Count > 0)
                    {
                        await responseStream.WriteAsync(queue.Dequeue()).ConfigureAwait(false);
                    }

                    autoResetEvent.WaitOne();
                }
            }
            finally
            {
                _logManager.Messages.CollectionChanged -= StateChanged;
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        IEnumerable<LogMessage> GetLogMessagesFrom(IEnumerable<string> messages)
        {
            return messages.Select(_ => new LogMessage
            {
                Message = _
            }).ToList();
        }
    }
}