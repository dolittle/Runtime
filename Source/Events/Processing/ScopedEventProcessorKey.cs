namespace Dolittle.Runtime.Events.Processing
{
    using Dolittle.Artifacts;
    using Dolittle.Concepts;
    using Dolittle.Tenancy;

    /// <summary>
    /// A key to mark the <see cref="Artifact">Event Artifact</see> and <see cref="TenantId">Tenant</see> scope 
    /// </summary>
    public class ScopedEventProcessorKey : Value<ScopedEventProcessorKey>
    {
        /// <summary>
        /// Instantiates an instance of the key
        /// </summary>
        public ScopedEventProcessorKey(TenantId tenant, Artifact @event)
        {
            Event = @event;
            Tenant = tenant;
        }
        /// <summary>
        /// The <see cref="Artifact"/> representing the Event
        /// </summary>
        /// <value></value>
        public Artifact Event { get; }

        /// <summary>
        /// The <see cref="TenantId">id</see> of the Tenant
        /// </summary>
        /// <value></value>
        public TenantId Tenant { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Event} - {Tenant}";
        } 
    }
}