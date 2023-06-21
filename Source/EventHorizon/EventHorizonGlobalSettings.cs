// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.EventHorizon;

[Configuration("eventHorizon")]
public class EventHorizonGlobalSettings
{
    public bool RequireConsent { get; set; } = true;
}
