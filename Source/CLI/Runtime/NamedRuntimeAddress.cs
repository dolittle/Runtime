// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime;

/// <summary>
/// Represents the address to a named instance of a Runtime for a Microservice.
/// </summary>
/// <param name="Name">The name of the Microservice.</param>
/// <param name="Host">The host of a microservice.</param>
/// <param name="Port">The host of a microservice.</param>
public record NamedRuntimeAddress(
    MicroserviceName Name,
    MicroserviceHost Host,
    MicroservicePort Port);
