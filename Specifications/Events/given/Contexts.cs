// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Applications;
using Dolittle.Execution;
using Dolittle.Security;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Specs.given
{
    public static class Contexts
    {
        public static readonly Application application = Guid.NewGuid();
        public static readonly BoundedContext bounded_context = Guid.NewGuid();
        public static readonly TenantId tenant = Guid.NewGuid();
        public static readonly TenantId another_tenant = Guid.NewGuid();

        public static ExecutionContext get_execution_context()
        {
            return new ExecutionContext(application, bounded_context, tenant, "Development", CorrelationId.New(), new Claims(new List<Claim>()), System.Threading.Thread.CurrentThread.CurrentCulture);
        }
    }
}