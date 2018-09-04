namespace Dolittle.Runtime.Events.Processing
{
    using Dolittle.Artifacts;
    using Dolittle.Concepts;
    using Dolittle.Runtime.Tenancy;

    public class ScopedEventProcessorKey : Value<ScopedEventProcessorKey>
    {
        public ScopedEventProcessorKey(TenantId tenant, Artifact @event)
        {
            Event = @event;
            Tenant = tenant;
        }
        public Artifact Event { get; }
        public TenantId Tenant { get; }

        public override string ToString()
        {
            return $"{Event} - {Tenant}";
        } 
    }
}