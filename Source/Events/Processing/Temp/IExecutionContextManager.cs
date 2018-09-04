#pragma warning disable 1591

namespace Dolittle.Runtime.Events.Processing
{
    using System.Security.Claims;
    using Dolittle.Applications;
    using Dolittle.Execution;
    using Dolittle.Runtime.Tenancy;

    public interface IExecutionContextManager
    {
        IExecutionContext Current { get; set; }

        void SetConstants(Application application, BoundedContext boundedContext);

        IExecutionContext CurrentFor(TenantId tenant);

        IExecutionContext CurrentFor(TenantId tenant, CorrelationId correlationId, ClaimsPrincipal principal = null);
    }
}