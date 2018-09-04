#pragma warning disable 1591

namespace Dolittle.Runtime.Events.Processing
{
    using System.Globalization;
    using System.Security.Claims;
    using Dolittle.Applications;
    using Dolittle.Execution;
    using Dolittle.Runtime.Tenancy;

    public interface IExecutionContext
    {
        /// <summary>
        /// Gets the <see cref="Application"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        Application Application { get; }

        /// <summary>
        /// Gets the <see cref="BoundedContext"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        BoundedContext BoundedContext { get; }

        /// <summary>
        /// Gets the <see cref="TenantId"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        TenantId Tenant { get; }

        /// <summary>
        /// Gets the <see cref="CorrelationId"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        ClaimsPrincipal Principal { get; }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        CultureInfo Culture { get; }
    }
}