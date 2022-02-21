// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Execution;

/// <summary>
/// Represents the configuration of the Runtime context from the Platform.
/// </summary>
/// <param name="CustomerID">The unique identifier of the customer the Runtime is executing for.</param>
/// <param name="ApplicationID">The unique identifier of the application the Runtime is executing for.</param>
/// <param name="MicroserviceID">The unique identifier of the microservice the Runtime is executing for.</param>
/// <param name="CustomerName">The name of the customer the Runtime is executing for.</param>
/// <param name="ApplicationName">The name of the application the Runtime is executing for.</param>
/// <param name="MicroserviceName">The name of the microservice the Runtime is executing for.</param>
/// <param name="Environment">The name of the environment the Runtime is executing for.</param>
[Configuration("platform")]
public record PlatformConfiguration(
    CustomerId CustomerID,
    ApplicationId ApplicationID,
    MicroserviceId MicroserviceID,
    CustomerName CustomerName,
    ApplicationName ApplicationName,
    MicroserviceName MicroserviceName,
    Environment Environment);
