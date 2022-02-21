// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Security;
using Dolittle.Runtime.Versioning;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Execution;

/// <summary>
/// Represents an implementation of <see cref="ICreateExecutionContexts"/>.
/// </summary>
[Singleton]
public class ExecutionContextCreator : ICreateExecutionContexts
{
    readonly PlatformConfiguration _configuration;

    public ExecutionContextCreator(IOptions<PlatformConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    /// <inheritdoc />
    public ExecutionContext CreateFor(TenantId tenant, Version version, CorrelationId correlation, Claims claims)
        => new(
            _configuration.MicroserviceID,
            tenant,
            version,
            _configuration.Environment,
            correlation,
            claims,
            CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public Try<ExecutionContext> TryCreateUsing(ExecutionContext requested)
    {
        if (requested.Microservice != _configuration.MicroserviceID)
        {
            return new InvalidExecutionContext(
                nameof(ExecutionContext.Microservice),
                _configuration.MicroserviceID.ToString(),
                requested.Microservice.ToString());
        }
        
        if (requested.Environment != _configuration.Environment)
        {
            return new InvalidExecutionContext(
                nameof(ExecutionContext.Environment),
                _configuration.Environment.ToString(),
                requested.Environment.ToString());
        }

        return requested;
    }
}
