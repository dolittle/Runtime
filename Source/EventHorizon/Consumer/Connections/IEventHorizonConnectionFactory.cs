// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections;

/// <summary>
/// Defines a system that can create instances of <see cref="IEventHorizonConnection"/>.
/// </summary>
public interface IEventHorizonConnectionFactory
{
    /// <summary>
    /// Creates a new <see cref="IEventHorizonConnection"/> to another Runtime.
    /// </summary>
    /// <param name="connectionAddress">The address of the other microservices Runtime to connect to.</param>
    /// <returns>A new <see cref="IEventHorizonConnection"/> that is ready to be connected.</returns>
    IEventHorizonConnection Create(MicroserviceAddress connectionAddress);
}