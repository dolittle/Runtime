#pragma warning disable 1591

namespace Dolittle.Runtime.Events.Processing
{
    using System.Globalization;
    using System.Security.Claims;
    using System.Threading;
    using System;
    using Dolittle.Applications;
    using Dolittle.Execution;
    using Dolittle.Runtime.Execution;
    using Dolittle.Runtime.Tenancy;

    public class ExecutionContextManager : IExecutionContextManager
    {
        static AsyncLocal<IExecutionContext> _executionContext = new AsyncLocal<IExecutionContext>();

        Application _application;
        BoundedContext _boundedContext;

        /// <inheritdoc/>
        public IExecutionContext Current
        {
            get
            {
                var context = _executionContext.Value;
                if (context == null)throw new Exception("Execution Context not set");
                return context;
            }
            set { _executionContext.Value = value; }
        }

        /// <inheritdoc/>
        public void SetConstants(Application application, BoundedContext boundedContext)
        {
            _application = application;
            _boundedContext = boundedContext;
        }

        /// <inheritdoc/>
        public IExecutionContext CurrentFor(TenantId tenant)
        {
            return CurrentFor(tenant, CorrelationId.New(), new ClaimsPrincipal());
        }

        /// <inheritdoc/>
        public IExecutionContext CurrentFor(TenantId tenant, CorrelationId correlationId, ClaimsPrincipal principal = null)
        {
            var executionContext = new ExecutionContext(
                _application,
                _boundedContext,
                tenant,
                correlationId,
                principal ?? new ClaimsPrincipal(),
                CultureInfo.CurrentCulture);

            Current = executionContext;

            return executionContext;
        }
    }
}