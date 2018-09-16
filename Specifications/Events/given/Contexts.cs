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
    using Dolittle.Tenancy;
    using Dolittle.Security;

    public static class Contexts
    {
        public static readonly Application application = Guid.NewGuid();
        public static readonly BoundedContext bounded_context = Guid.NewGuid();
        public static readonly TenantId tenant = Guid.NewGuid();
        public static readonly TenantId another_tenant = Guid.NewGuid();

        public static ExecutionContext get_execution_context()
        {
            return new ExecutionContext(application,bounded_context,tenant,"Development",CorrelationId.New(),new Claims(new List<Claim>()),System.Threading.Thread.CurrentThread.CurrentCulture);
        }
    }
}