// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microservices;

/// <summary>
/// Represents the address of a Microservice.
/// </summary>
/// <param name="Host">The host of a microservice.</param>
/// <param name="Port">The host of a microservice.</param>
public record MicroserviceAddress(string Host, int Port);
