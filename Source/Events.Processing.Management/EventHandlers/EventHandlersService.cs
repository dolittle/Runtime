// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.EventHandlers;
using Artifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers
{
    public class EventHandlersService : EventHandlersBase
    {
        readonly IEventHandlers _eventHandlers;
        readonly IExceptionToFailureConverter _exceptionToFailureConverter;
        readonly ILogger _logger;
        
        public EventHandlersService(
            IEventHandlers eventHandlers,
            IExceptionToFailureConverter exceptionToFailureConverter,
            ILogger logger)
        {
            _eventHandlers = eventHandlers;
            _exceptionToFailureConverter = exceptionToFailureConverter;
            _logger = logger;
        }

        /// <inheritdoc />
        public override async Task<ReprocessEventsFromResponse> ReprocessEventsFrom(ReprocessEventsFromRequest request, ServerCallContext context)
        {
            var response = new ReprocessEventsFromResponse();
            var eventHandler = new EventHandlerId(request.ScopeId.ToGuid(), request.EventHandlerId.ToGuid());
            TenantId tenant = request.TenantId.ToGuid(); 
            _logger.ReprocessEventsFrom(eventHandler, tenant, request.StreamPosition);
            var reprocessing = await _eventHandlers.ReprocessEventsFrom(eventHandler, tenant, request.StreamPosition).ConfigureAwait(false);
            if (!reprocessing.Success)
            {
                response.Failure = _exceptionToFailureConverter.ToFailure(reprocessing.Exception);
            }
            
            return response;
        }

        /// <inheritdoc />
        public override async Task<ReprocessAllEventsResponse> ReprocessAllEvents(ReprocessAllEventsRequest request, ServerCallContext context)
        {
            var response = new ReprocessAllEventsResponse();
            var eventHandler = new EventHandlerId(request.ScopeId.ToGuid(), request.EventHandlerId.ToGuid());
            _logger.ReprocessAllEvents(eventHandler);
            var reprocessing = await _eventHandlers.ReprocessAllEvents(eventHandler).ConfigureAwait(false);
            if (!reprocessing.Success)
            {
                response.Failure = _exceptionToFailureConverter.ToFailure(reprocessing.Exception);
            }

            return response;
        }

        /// <inheritdoc />
        public override Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            _logger.GetAll();
            var eventHandlerInfos = _eventHandlers.All;

            foreach (var info in eventHandlerInfos)
            {
                var status = new EventHandlerStatus
                {   
                    Alias = info.Alias,
                    Partitioned = info.Partitioned,
                    ScopeId = info.Id.Scope.ToProtobuf(),
                    EventHandlerId = info.Id.EventHandler.ToProtobuf()
                };
                status.EventTypes.AddRange(CreateEventTypes(info));
                status.Tenants.AddRange(CreateScopedStreamProcessorStatus(info));
                
                response.EventHandlers.Add(status);
            }

            return Task.FromResult(response);
        }

        IEnumerable<Artifact> CreateEventTypes(EventHandlerInfo info)
            => info.EventTypes.Select(_ => new Artifact()
            {
                Id = _.ToProtobuf(),
                Generation = ArtifactGeneration.First,
            });
        
        IEnumerable<TenantScopedStreamProcessorStatus> CreateScopedStreamProcessorStatus(EventHandlerInfo info)
        {
            var state = _eventHandlers.CurrentStateFor(info.Id);
            if (!state.Success)
            {
                throw state.Exception;
            }

            return state.Result.Select(CreateStatusFromState);
        }

        static TenantScopedStreamProcessorStatus CreateStatusFromState(KeyValuePair<TenantId, IStreamProcessorState> tenantAndState)
        {
            var (tenant, state) = tenantAndState;
            var status = new TenantScopedStreamProcessorStatus
            {
                StreamPosition = state.Position,
                TenantId = tenant.ToProtobuf(),
            };

            switch (state)
            {
                case Streams.Partitioned.StreamProcessorState partitionedState:
                {
                    status.LastSuccessfullyProcessed = partitionedState.LastSuccessfullyProcessed.ToTimestamp();
                    status.Partitioned = new PartitionedTenantScopedStreamProcessorStatus();
                    foreach (var (partition, failure) in partitionedState.FailingPartitions)
                    {
                        status.Partitioned.FailingPartitions.Add(
                            new FailingPartition
                            {
                                PartitionId = partition,
                                FailureReason = failure.Reason,
                                LastFailed = failure.LastFailed.ToTimestamp(),
                                RetryCount = failure.ProcessingAttempts,
                                RetryTime = failure.RetryTime.ToTimestamp(),
                                StreamPosition = failure.Position
                            });
                    }

                    break;
                }
                case Streams.StreamProcessorState unpartitionedState:
                    status.LastSuccessfullyProcessed = unpartitionedState.LastSuccessfullyProcessed.ToTimestamp();
                    status.Unpartitioned = new UnpartitionedTenantScopedStreamProcessorStatus
                    {
                        FailureReason = unpartitionedState.FailureReason,
                        IsFailing = unpartitionedState.IsFailing,
                        RetryCount = unpartitionedState.ProcessingAttempts,
                        RetryTime = unpartitionedState.RetryTime.ToTimestamp()
                    };
                    break;
            }
            
            return status;
        }
    }
}