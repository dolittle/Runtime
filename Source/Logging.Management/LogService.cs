// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Logging.Management;
using Dolittle.Logging.Json;
using Dolittle.Protobuf;
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
        public override async Task Listen(Empty request, IServerStreamWriter<LogMessages> responseStream, ServerCallContext context)
        {
            using var autoResetEvent = new AutoResetEvent(false);

            void StateChanged(object sender, NotifyCollectionChangedEventArgs args)
            {
                if (args.NewItems != default)
                {
                    var items = new List<JsonLogMessage>();
                    foreach (var item in args.NewItems)
                    {
                        items.Add(item as JsonLogMessage);
                    }

                    var messages = GetLogMessagesFrom(items);
                    responseStream.WriteAsync(messages).Wait();
                }
            }

            try
            {
                _logManager.Messages.CollectionChanged += StateChanged;

                context.CancellationToken.ThrowIfCancellationRequested();

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(GetLogMessagesFrom(_logManager.Messages)).ConfigureAwait(false);
                    autoResetEvent.WaitOne();
                }
            }
            finally
            {
                _logManager.Messages.CollectionChanged += StateChanged;
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        LogMessages GetLogMessagesFrom(IEnumerable<JsonLogMessage> messages)
        {
            var logMessages = new LogMessages();
            var converted = messages.Select(_ => new LogMessage
            {
                Application = _.Application.ToProtobuf(),
                Microservice = _.Microservice.ToProtobuf(),
                Tenant = _.TenantId.ToProtobuf(),
                CorrelationId = _.CorrelationId.ToProtobuf(),
                Environment = _.Environment,
                Timestamp = Timestamp.FromDateTimeOffset(_.Timestamp),
                LogLevel = _.LogLevel,
                FilePath = _.FilePath,
                LineNumber = _.LineNumber,
                Member = _.Member,
                Message = _.Message,
                StackTrace = _.StackTrace
            });

            logMessages.Messages.Add(converted);
            return logMessages;
        }
    }
}