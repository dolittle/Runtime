#pragma warning disable 1591

namespace Dolittle.Runtime.Events.Processing
{
    using System.Globalization;
    using System.Security.Claims;
    using Dolittle.Applications;
    using Dolittle.Execution;
    using Dolittle.Runtime.Tenancy;

    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(
            Application application,
            BoundedContext boundedContext,
            TenantId tenant,
            CorrelationId correlationId,
            ClaimsPrincipal principal,
            CultureInfo cultureInfo)
        {
            Application = application;
            BoundedContext = boundedContext;
            Tenant = tenant;
            CorrelationId = correlationId;
            Principal = principal;
            Culture = cultureInfo;
        }

        /// <inheritdoc/>
        public Application Application { get; }

        /// <inheritdoc/>
        public BoundedContext BoundedContext { get; }

        /// <inheritdoc/>
        public TenantId Tenant { get; }

        /// <inheritdoc/>
        public CorrelationId CorrelationId { get; }

        /// <inheritdoc/>
        public ClaimsPrincipal Principal { get; }

        /// <inheritdoc/>
        public CultureInfo Culture { get; }
    }
}