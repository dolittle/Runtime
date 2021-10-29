// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.Replay
{
    /// <summary>
    /// A shared command base for the "dolittle runtime eventhandlers replay" commands that provides shared arguments.
    /// </summary>
    public abstract class CommandBase : EventHandlers.CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
        protected CommandBase(ICanLocateRuntimes runtimes, IResolveEventHandlerId eventHandlerIdResolver, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
            : base(runtimes, eventHandlerIdResolver, eventTypesDiscoverer,  jsonSerializer)
        {
        }
        
        /// <summary>
        /// The Event Handler identifier argument used to provide the unique identifier of the Event Handler to replay.
        /// </summary>
        [Required]
        [Argument(0, Description = "The Event Handler identifier of the Event Handler to replay")]
        protected EventHandlerIdOrAlias EventHandlerIdentifier { get; init; }
    }
}
