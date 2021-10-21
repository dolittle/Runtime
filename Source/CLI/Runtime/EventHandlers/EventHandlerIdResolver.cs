using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Represents an implementation of the <see cref="IResolveEventHandlerId"/>.  
    /// </summary>
    public class EventHandlerIdResolver : IResolveEventHandlerId
    {
        readonly IManagementClient _managementClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerIdResolver"/> class.
        /// </summary>
        /// <param name="managementClient">The <see cref="IManagementClient"/>.</param>
        public EventHandlerIdResolver(IManagementClient managementClient)
        {
            _managementClient = managementClient;
        }

        /// <inheritdoc />
        public async Task<EventHandlerId> ResolveId(MicroserviceAddress runtime, EventHandlerIdOrAlias idOrAlias)
        {
            if (!idOrAlias.IsAlias)
            {
                return idOrAlias.Id;
            }
            var statuses = await _managementClient.GetAll(runtime).ConfigureAwait(false);
            var status = statuses.FirstOrDefault(_ => WithAlias(_, idOrAlias));
            
            if (status == default)
            {
                throw new NoEventHandlerWithId(idOrAlias.Alias, idOrAlias.Scope);
            }
            return status.Id;
        }

        static bool WithAlias(EventHandlerStatus status, EventHandlerIdOrAlias idOrAlias)
            => status.HasAlias && status.Alias.Equals(idOrAlias.Alias) && status.Id.Scope.Equals(idOrAlias.Scope);
    }
}
