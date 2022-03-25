// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.EventHorizon.Configuration;

/// <summary>
/// Represents the event horizons configuration for a tenant.
/// </summary>
[TenantConfiguration("eventHorizons")]
public class EventHorizonsPerMicroserviceConfiguration : Dictionary<Guid, EventHorizonConfiguration>
{
}
