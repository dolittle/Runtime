// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents the platform information about the running microservice.
/// </summary>
/// <param name="CustomerId">The <see cref="CustomerId"/> id of the microservice.</param>
/// <param name="ApplicationId">The <see cref="ApplicationId"/> id of the microservice.</param>
/// <param name="MicroserviceId">The <see cref="MicroserviceId"/> id of the microservice.</param>
/// <param name="CustomerName">The <see cref="CustomerName"/> id of the microservice.</param>
/// <param name="ApplicationName">The <see cref="ApplicationName"/> id of the microservice.</param>
/// <param name="MicroserviceName">The <see cref="MicroserviceName"/> id of the microservice.</param>
/// <param name="Environment">The <see cref="Environment"/> of the microservice.</param>
public record PlatformEnvironment(
    CustomerId CustomerId,
    ApplicationId ApplicationId,
    MicroserviceId MicroserviceId,
    CustomerName CustomerName,
    ApplicationName ApplicationName,
    MicroserviceName MicroserviceName,
    Environment Environment);
