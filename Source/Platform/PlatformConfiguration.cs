// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Configuration;
using Dolittle.Runtime.Domain.Platform;
using ApplicationId = Dolittle.Runtime.Domain.Platform.ApplicationId;

namespace Dolittle.Runtime.Platform;

/// <summary>
/// Represents the configuration of the Runtime context from the Platform.
/// </summary>
[Configuration("platform")]
public class PlatformConfiguration
{
    /// <summary>The unique identifier of the customer the Runtime is executing for.</summary>
    public Guid CustomerID { get; init; } = CustomerId.NotSet;

    /// <summary>The unique identifier of the application the Runtime is executing for.</summary>
    public Guid ApplicationID { get; init; } = ApplicationId.NotSet;

    /// <summary>The unique identifier of the microservice the Runtime is executing for.</summary>
    public Guid MicroserviceID { get; init; } = MicroserviceId.NotSet;

    /// <summary>The name of the customer the Runtime is executing for.</summary>
    public string CustomerName { get; init; } = Domain.Platform.CustomerName.NotSet;

    /// <summary>The name of the application the Runtime is executing for.</summary>
    public string ApplicationName { get; init; } = Domain.Platform.ApplicationName.NotSet;

    /// <summary>The name of the microservice the Runtime is executing for.</summary>
    public string MicroserviceName { get; init; } = Domain.Platform.MicroserviceName.NotSet;

    /// <summary>The name of the environment the Runtime is executing for.</summary>
    public string Environment { get; init; } = Domain.Platform.Environment.Development;
}
