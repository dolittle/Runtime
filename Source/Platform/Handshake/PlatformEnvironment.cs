// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents the platform information about the running Microservice.
/// </summary>
/// <param name="MicroserviceId">The <see cref="MicroserviceId"/> id of the Runtime.</param>
/// <param name="Environment">The <see cref="Execution.Environment"/> of the Microservice.</param>
public record PlatformEnvironment(MicroserviceId MicroserviceId, Execution.Environment Environment);
