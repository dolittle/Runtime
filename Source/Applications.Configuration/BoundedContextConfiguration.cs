// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Applications.Configuration;

/// <summary>
/// Represents the definition of a <see cref="MicroserviceId"/> for configuration.
/// </summary>
[Name("bounded-context")]
public record BoundedContextConfiguration(
    Guid Application,
    Guid BoundedContext,
    string BoundedContextName,
    CoreConfiguration Core,
    IEnumerable<InteractionLayerConfiguration> Interaction,
    IDictionary<string, ResourceTypeImplementationConfiguration> Resources) : IConfigurationObject;