// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;

/// <summary>
/// Represents the configuration for microservices.
/// </summary>
[Name("microservices")]
public class MicroservicesConfiguration : Dictionary<Guid, MicroserviceAddressConfiguration>, IConfigurationObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MicroservicesConfiguration"/> class.
    /// </summary>
    /// <param name="configuration">Dictionary for <see cref="MicroserviceId"/> with <see cref="MicroserviceAddress"/>.</param>
    public MicroservicesConfiguration(IDictionary<Guid, MicroserviceAddressConfiguration> configuration)
        : base(configuration)
    {
    }
}
