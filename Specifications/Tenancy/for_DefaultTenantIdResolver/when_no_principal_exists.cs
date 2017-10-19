using System.Security.Claims;
using Machine.Specifications;

namespace doLittle.Runtime.Tenancy.Specs.for_DefaultTenantIdResolver
{
    public class when_no_principal_exists : given.a_default_tenant_id_resolver
    {
        static TenantId result;

        Establish context = () => ClaimsPrincipal.ClaimsPrincipalSelector = () => null;

        Because of = () => result = resolver.Resolve();

        It should_return_unknown_tenant = () => result.Value.ShouldEqual(DefaultTenantIdResolver.UnknownTenantId);
    }
}
