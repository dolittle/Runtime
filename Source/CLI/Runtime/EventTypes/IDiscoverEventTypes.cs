// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes;

/// <summary>
/// Defines a system that discovers Event Types in the Runtime.
/// </summary>
public interface IDiscoverEventTypes
{
    /// <summary>
    /// Gets the discovered Event Types in the Runtime.
    /// </summary>
    /// <param name="runtime">The Runtime microservice address.</param>
    /// <returns>The discovered Event Types.</returns>
    Task<IEnumerable<EventType>> Discover(MicroserviceAddress runtime);
}