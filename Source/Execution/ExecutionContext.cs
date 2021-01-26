// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Security;
using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Represents a <see cref="ExecutionContext"/>.
    /// </summary>
    public record ExecutionContext(
        Microservice Microservice,
        TenantId Tenant,
        Version Version,
        Environment Environment,
        CorrelationId CorrelationId,
        Claims Claims,
        CultureInfo CultureInfo);
}
