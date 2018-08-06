/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Globalization;
using System.Security.Claims;
using Dolittle.Applications;
using Dolittle.Dynamic;
using Dolittle.Runtime.Tenancy;
using Dolittle.DependencyInversion.Conventions;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Represents a <see cref="IExecutionContext"/>
    /// </summary>
    public class ExecutionContext : IExecutionContext
    {
        /// <summary>
        /// Initializes an instance of <see cref="ExecutionContext"/>
        /// </summary>
        /// <param name="principal"><see cref="ClaimsPrincipal"/> to populate with</param>
        /// <param name="cultureInfo"><see cref="CultureInfo"/> for the <see cref="ExecutionContext"/></param>
        /// <param name="detailsPopulator">Callback that gets called for populating the details of the <see cref="ExecutionContext"/></param>
        /// <param name="application"><see cref="Application"/> that is currently executing</param>
        /// <param name="boundedContext"><see cref="BoundedContext"/> that is currently executing</param>
        /// <param name="tenant"><see cref="ITenant"/> that is currently part of the <see cref="IExecutionContext"/></param>
        public ExecutionContext(
            ClaimsPrincipal principal,
            CultureInfo cultureInfo,
            ExecutionContextPopulator detailsPopulator,
            Application application,
            BoundedContext boundedContext,
            ITenant tenant)
        {
            Principal = principal;
            Culture = cultureInfo;
            Application = application;
            BoundedContext = boundedContext;
            Tenant = tenant;
            Details = new WriteOnceExpandoObject(d => detailsPopulator(this,d));
        }

        /// <inheritdoc/>
        public ClaimsPrincipal Principal { get; }

        /// <inheritdoc/>
        public CultureInfo Culture { get; }

        /// <inheritdoc/>
        public Application Application { get; }

        /// <inheritdoc/>
        public BoundedContext BoundedContext { get; }

        /// <inheritdoc/>
        public ITenant Tenant { get; }

        /// <inheritdoc/>
        public dynamic Details { get; }
    }
}
