// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Serialization.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers.Replay
{
    /// <summary>
    /// A shared command base for the "dolittle runtime eventhandlers replay" commands that provides shared arguments.
    /// </summary>
    public abstract class CommandBase : Runtime.CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
        protected CommandBase(ICanLocateRuntimes runtimes, ISerializer jsonSerializer)
            : base(runtimes, jsonSerializer)
        {
        }
        
        /// <summary>
        /// The "--id" argument used to provide the identifier of the Event Handler to replay.
        /// </summary>
        [Required]
        [Option("--id", CommandOptionType.SingleValue, Description = "The ID of the Event Handler to replay")]
        protected EventProcessorId Identifier { get; init; }
        
        [Option("--scope", CommandOptionType.SingleValue, Description = "The Scope of the Event Handler to replay. Defaults to the default scope.")]
        ScopeId Scope { get; init; }

        /// <summary>
        /// The "--scope" argument used to provide the scope of the Event Handler to replay, or <see cref="ScopeId.Default"/>.
        /// </summary>
        protected ScopeId SpecifiedScopeOrDefault => Scope ?? ScopeId.Default;

        /// <summary>
        /// The identifier for the Event Handler to replay, using "--id" and "--scope".
        /// </summary>
        protected EventHandlerId EventHandler => new EventHandlerId(SpecifiedScopeOrDefault, Identifier);
    }
}
