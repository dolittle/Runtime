using System;

namespace Dolittle.Runtime.EventHorizon.Configuration;

public class SubscriptionConfiguration
{
    public Guid Stream { get; set; }
    public string Partition { get; set; }
    public Guid Scope { get; set; }
}