using System;
using System.Globalization;
using System.Linq;
using Dolittle.Applications;
using Dolittle.Concepts;
using Dolittle.Execution;
using Dolittle.Runtime.Events.Store;
using Dolittle.Security;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents the origin (<see cref="Application"/>, <see cref="BoundedContext"/>, <see cref="TenantId">Tenant</see>, <see cref="Environment" /> and <see cref="Claims">User Claims</see> ) from the <see cref="ExecutionContext"/>
    /// where the Event was created
    /// </summary>
    public class OriginalContext : Value<OriginalContext>
    {
        /// <summary>
        /// Initializes an instance of <see cref="OriginalContext"/>
        /// </summary>
        /// <param name="application"><see cref="Applications.Application"/> that is the source of the event</param>
        /// <param name="boundedContext"><see cref="BoundedContext"/> that is the source of the event</param>
        /// <param name="tenant"><see cref="TenantId"/> that is related to the source of the event</param>
        /// <param name="environment"><see cref="Dolittle.Execution.Environment"/> for the original <see cref="ExecutionContext"/></param>
        /// <param name="claims"><see cref="Claims"/> for the user who initiated the event</param>
        /// <param name="commitSequenceNumber"><see cref="CommitSequenceNumber"/> for the commit of which this event is part.  May not be populated in the source Bounded Context.</param>
        public OriginalContext(
            Dolittle.Applications.Application application,
            BoundedContext boundedContext,
            TenantId tenant,
            Dolittle.Execution.Environment environment,
            Claims claims,
            CommitSequenceNumber commitSequenceNumber = null)
        {
            Application = application;
            BoundedContext = boundedContext;
            Tenant = tenant;
            Environment = environment;
            Claims = claims;
            CommitInOrigin = commitSequenceNumber ?? 0;
        }

        /// <summary>
        /// Gets the <see cref="Application"/> for the <see cref="ExecutionContext">execution context</see>
        /// </summary>
        public Dolittle.Applications.Application Application { get; }

        /// <summary>
        /// Gets the <see cref="BoundedContext"/> for the <see cref="ExecutionContext">execution context</see>
        /// </summary>
        public BoundedContext BoundedContext { get; }

        /// <summary>
        /// Gets the <see cref="TenantId"/> for the <see cref="ExecutionContext">execution context</see>
        /// </summary>
        public TenantId Tenant { get; }

        /// <summary>
        /// Gets the <see cref="Environment"/> for the <see cref="ExecutionContext">execution context</see>
        /// </summary>
        public Dolittle.Execution.Environment Environment { get; }

        /// <summary>
        /// Gets the <see cref="Claims"/> for the <see cref="ExecutionContext">execution context</see>
        /// </summary>
        public Claims Claims { get; }

        /// <summary>
        /// Add the Commit so we can populate it when sending events to another Bounded Context;
        /// </summary>
        /// <value></value>
        public CommitSequenceNumber CommitInOrigin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executionContext"></param>
        public static implicit operator OriginalContext(ExecutionContext executionContext)
        {
            return new OriginalContext(
                executionContext.Application ?? Guid.Empty,
                executionContext.BoundedContext ?? Guid.Empty,
                executionContext.Tenant ?? Guid.Empty,
                executionContext.Environment ?? string.Empty,
                executionContext.Claims ?? new Claims(Enumerable.Empty<Dolittle.Security.Claim>())
            );
        }

        /// <summary>
        /// Converts this into an <see cref="ExecutionContext" />
        /// </summary>
        /// <param name="correlationId">The correlation Id for this ExecutionContext</param>
        /// <returns></returns>
        public ExecutionContext ToExecutionContext(CorrelationId correlationId)
        {
            return new ExecutionContext(Application,BoundedContext,Tenant,Environment,correlationId,Claims,CultureInfo.CurrentCulture);
        }
    }
}