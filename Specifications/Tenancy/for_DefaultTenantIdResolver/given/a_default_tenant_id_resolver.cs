using Machine.Specifications;

namespace Dolittle.Runtime.Tenancy.Specs.for_DefaultTenantIdResolver.given
{
    public class a_default_tenant_id_resolver
    {
        protected static DefaultTenantIdResolver resolver;

        Establish context = () => resolver = new DefaultTenantIdResolver();
    }
}
