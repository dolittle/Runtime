// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Linq;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Google.Protobuf;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the implementation of.
    /// </summary>
    public class FiltersService : FiltersBase
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FiltersService(
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task Connect(IAsyncStreamReader<FilterClientToRuntimeResponse> requestStream, IServerStreamWriter<FilterRuntimeToClientRequest> responseStream, ServerCallContext context)
        {
            var filterId = ByteString.CopyFrom(context.RequestHeaders.First(_ => _.Key == $"filterid{Metadata.BinaryHeaderSuffix}").ValueBytes).To<EventProcessorId>();
            var streamId = ByteString.CopyFrom(context.RequestHeaders.First(_ => _.Key == $"streamid{Metadata.BinaryHeaderSuffix}").ValueBytes).To<StreamId>();
            _logger.Information($"Filter client connected - '{filterId}' - '{streamId}' - Method: {context.Method}");

            Task.Run(async () =>
            {
                while (await requestStream.MoveNext(context.CancellationToken).ConfigureAwait(false))
                {
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    _logger.Information($"Message received");
                }
            }).ConfigureAwait(false);

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    _logger.Information("Send to client");
                    var filter = new FilterRuntimeToClientRequest();

                    var current = _executionContextManager.Current;

                    _logger.Information($"Tenant {current.Tenant} - Correlation {current.CorrelationId}");

                    var currentMessage = new Execution.Contracts.ExecutionContext
                    {
                        Microservice = current.BoundedContext.ToProtobuf(),
                        Tenant = current.Tenant.ToProtobuf(),
                        CorrelationId = current.CorrelationId.ToProtobuf(),
                    };

                    currentMessage.Claims.AddRange(current.Claims.Select(_ => new Security.Contracts.Claim
                    {
                        Key = _.Name,
                        Value = _.Value,
                        ValueType = _.ValueType
                    }));

                    filter.ExecutionContext = currentMessage.ToByteString();

                    await responseStream.WriteAsync(filter).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

            context.CancellationToken.ThrowIfCancellationRequested();
            context.CancellationToken.WaitHandle.WaitOne();

            _logger.Information($"Filter client disconnected");

            return Task.CompletedTask;
        }
    }
}