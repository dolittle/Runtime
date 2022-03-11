// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.EventHorizon.Configuration;

/// <summary>
/// Represents the configuration for event horizon consents per consumer tenant.
/// </summary>
public class ConsentsPerConsumerConfiguration : Dictionary<Guid, ConsentConfiguration>
{
}
