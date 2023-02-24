// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using Dolittle.Runtime.Domain.Tenancy;
// using Dolittle.Runtime.Events.Processing.Streams.Actors;
// using Dolittle.Runtime.Events.Store.Streams;
// using Dolittle.Runtime.Rudimentary;
// using Dolittle.Runtime.Tenancy;
// using Proto;
//
// namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;
//
//
//
// public class EventHandlerClient: IEventHandler
// {
//     readonly ActorSystem _actorSystem;
//     readonly ITenants _tenants;
//     readonly PID _kindPid;
//
//     public EventHandlerClient(PID kindPid, ActorSystem actorSystem, ITenants tenants)
//     {
//         _kindPid = kindPid;
//         _actorSystem = actorSystem;
//         _tenants = tenants;
//     }
//
//     public EventHandlerInfo Info { get; }
//
//     public Task<Try<IDictionary<TenantId, IStreamProcessorState>>> GetEventHandlerCurrentState() => Try<IDictionary<TenantId, IStreamProcessorState>>.DoAsync(async () =>
//     {
//         var response = await _actorSystem.Root.RequestAsync<IDictionary<TenantId, IStreamProcessorState>>(_kindPid, new GetCurrentProcessorState(),
//             CancellationToken.None);
//         return response;
//     });
//
//     public Task<Try<ProcessingPosition>> ReprocessEventsFrom(TenantId tenant, ProcessingPosition position) => Try<ProcessingPosition>.DoAsync(async () =>
//     {
//         var response = await _actorSystem.Root.RequestAsync<ProcessingPosition>(_kindPid, new ReprocessEventsFrom(tenant,position),
//             CancellationToken.None);
//         return response;
//     });
//
//     public Task<Try<IDictionary<TenantId, Try<ProcessingPosition>>>> ReprocessAllEvents() => Try<IDictionary<TenantId, Try<ProcessingPosition>>>.DoAsync(async () =>
//     {
//         var results = new Dictionary<TenantId, Try<ProcessingPosition>>();
//         
//         foreach (var tenantId in _tenants.All)
//         {
//             var result = await _actorSystem.Root.RequestAsync<ProcessingPosition>(_kindPid, new ReprocessEventsFrom(tenantId,ProcessingPosition.Initial),
//                 CancellationToken.None);
//             results.Add(tenantId, result);
//         }
//         
//         return results;
//     });
//
//     public Task RegisterAndStart() => throw new System.NotImplementedException();
//     
//     public void Dispose() => throw new System.NotImplementedException();
//
//     public event EventHandlerRegistrationFailed? OnRegistrationFailed;
// }
