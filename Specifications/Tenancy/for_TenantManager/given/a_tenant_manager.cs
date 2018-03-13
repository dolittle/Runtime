using Machine.Specifications;

namespace Dolittle.Runtime.Tenancy.Specs.for_TenantManager.given
{
    public class a_tenant_manager : all_dependencies
    {
        protected static TenantManager tenant_manager;

        Establish context = () => 
            tenant_manager = 
                new TenantManager(
                    call_context.Object,
                    tenant_populator.Object,
                    tenant_id_resolver.Object
                );
    }
}
