using System;

namespace Dolittle.Runtime.EventHorizon.Configuration;

public class ConsentConfiguration
{
    public Guid Stream { get; set; }
    public string Partition { get; set; }
    public Guid Consent { get; set; }
}
