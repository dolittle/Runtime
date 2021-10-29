// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Serialization.Json;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes
{
    /// <summary>
    /// A shared command base for the "dolittle runtime eventtypes" commands that provides shared arguments.
    /// </summary>
    public abstract class CommandBase : Runtime.CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase"/> class.
        /// </summary>
        /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
        /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
        protected CommandBase(ICanLocateRuntimes runtimes, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
            : base(runtimes, eventTypesDiscoverer, jsonSerializer)
        {
        }
    }
}
