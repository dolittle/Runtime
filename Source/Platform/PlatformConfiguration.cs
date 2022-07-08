// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;
using Dolittle.Runtime.Domain.Platform;

namespace Dolittle.Runtime.Platform;

/// <summary>
/// Represents the configuration of the Runtime context from the Platform.
/// </summary>
[Configuration("platform")]
public class PlatformConfiguration
{
    /// <summary>The unique identifier of the customer the Runtime is executing for.</summary>
    public CustomerId CustomerID { get; init; } = CustomerId.NotSet;

    /// <summary>The unique identifier of the application the Runtime is executing for.</summary>
    public ApplicationId ApplicationID { get; init; } = ApplicationId.NotSet;

    /// <summary>The unique identifier of the microservice the Runtime is executing for.</summary>
    public MicroserviceId MicroserviceID { get; init; } = MicroserviceId.NotSet;

    /// <summary>The name of the customer the Runtime is executing for.</summary>
    public CustomerName CustomerName { get; init; } = CustomerName.NotSet;

    /// <summary>The name of the application the Runtime is executing for.</summary>
    public ApplicationName ApplicationName { get; init; } = ApplicationName.NotSet;

    /// <summary>The name of the microservice the Runtime is executing for.</summary>
    public MicroserviceName MicroserviceName { get; init; } = MicroserviceName.NotSet;

    /// <summary>The name of the environment the Runtime is executing for.</summary>
    public Environment Environment { get; init; } = Environment.Development;
}
