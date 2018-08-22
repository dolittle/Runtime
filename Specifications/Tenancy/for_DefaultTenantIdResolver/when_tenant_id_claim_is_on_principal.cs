using System;
using System.Security.Claims;
using Machine.Specifications;

namespace Dolittle.Runtime.Tenancy.Specs.for_DefaultTenantIdResolver
{
    public class when_tenant_id_claim_is_on_principal : given.a_default_tenant_id_resolver
    {
        static string tenant_id = Guid.NewGuid().ToString();
        static TenantId result;

        Establish context = () => ClaimsPrincipal.ClaimsPrincipalSelector = () =>
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(DefaultTenantIdResolver.TenantIdClaimType, tenant_id));
            var principal = new ClaimsPrincipal(identity);
            return principal;
        };

        Because of = () => result = resolver.Resolve();

        It should_return_tenant_id_in_claim = () => result.Value.ToString().ShouldEqual(tenant_id);
    }
}
