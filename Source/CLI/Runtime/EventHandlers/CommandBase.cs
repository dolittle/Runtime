// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Serialization.Json;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// A shared command base for the "dolittle runtime eventhandlers" commands that provides shared arguments.
    /// </summary>
    public abstract class CommandBase : Runtime.CommandBase
    {
        readonly IResolveEventHandlerId _eventHandlerIdResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="eventHandlerIdResolver">The Event Handler Id resolver.</param>
        /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
        protected CommandBase(ICanLocateRuntimes runtimes, IResolveEventHandlerId eventHandlerIdResolver, ISerializer jsonSerializer)
            : base(runtimes, jsonSerializer)
        {
            _eventHandlerIdResolver = eventHandlerIdResolver;
        }
        
        /// <summary>
        /// Gets the <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="runtime">The Runtime microservice address.</param>
        /// <param name="idOrAlias">The Event Handler Id or Alias.</param>
        /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="EventHandlerId"/>.</returns>
        protected Task<EventHandlerId> GetEventHandlerId(MicroserviceAddress runtime, EventHandlerIdOrAlias idOrAlias)
            => _eventHandlerIdResolver.ResolveId(runtime, idOrAlias);
    }
}
