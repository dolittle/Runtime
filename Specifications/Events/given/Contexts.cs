namespace Dolittle.Runtime.Events.Specs.given
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dolittle.Applications;
    using Dolittle.Artifacts;
    using Dolittle.Collections;
    using Dolittle.Events;
    using Dolittle.Execution;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Tenancy;

    public static class Contexts
    {
        public static readonly Application application = Guid.NewGuid();
        public static readonly BoundedContext bounded_context = Guid.NewGuid();
        public static readonly TenantId tenant = Guid.NewGuid();
        public static readonly TenantId another_tenant = Guid.NewGuid();

        public static IExecutionContext get_execution_context()
        {
            return new ExecutionContext(application,bounded_context,tenant,CorrelationId.New(),new System.Security.Claims.ClaimsPrincipal(),System.Threading.Thread.CurrentThread.CurrentCulture);
        }
    }
}