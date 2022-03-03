// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.EventHorizon.Configuration;

/// <summary>
/// Represents the consent for an event horizon.
/// </summary>
[TenantConfiguration("eventHorizons")]
public class EventHorizonsPerMicroserviceConfiguration : Dictionary<Guid, EventHorizonConfiguration>
{
}
