// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Threading;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Security;
using Dolittle.Runtime.Versioning;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Execution;

/// <summary>
/// Represents an implementation of <see cref="IExecutionContextManager"/>.
/// </summary>
[Singleton]
public class ExecutionContextManager : IExecutionContextManager
{
    static readonly AsyncLocal<ExecutionContext> _executionContext = new();

    static bool _initialExecutionContextSet;

    readonly ILogger _logger;
    Version _version;
    Environment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContextManager"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ExecutionContextManager(ILogger logger)
    {
        _logger = logger;
        _version = Version.NotSet;
        _environment = Environment.Undetermined;
    }

    /// <inheritdoc/>
    public ExecutionContext Current
    {
        get
        {
            var context = _executionContext.Value;
            if (context == null)
            {
                throw new ExecutionContextNotSet();
            }

            return context;
        }

        private set
        {
            _executionContext.Value = value;
        }
    }

    /// <summary>
    /// Set the initial <see cref="ExecutionContext"/>.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    /// <remarks>
    /// This can only be called once per process and is typically called by entrypoints into Dolittle itself.
    /// </remarks>
    /// <returns>An <see cref="ExecutionContext"/> instance.</returns>
    public static ExecutionContext SetInitialExecutionContext(ILogger logger)
    {
        Log.SettingInitialExecutionContext(logger);
        if (_initialExecutionContextSet)
        {
            throw new InitialExecutionContextHasAlreadyBeenSet();
        }

        _initialExecutionContextSet = true;

        _executionContext.Value = new ExecutionContext(
            Microservice.NotSet,
            TenantId.System,
            Version.NotSet,
            Environment.Undetermined,
            CorrelationId.System,
            Claims.Empty,
            CultureInfo.InvariantCulture);

        return _executionContext.Value;
    }

    /// <inheritdoc/>
    public void SetConstants(
        Microservice microservice,
        Version version,
        Environment environment,
        string filePath,
        int lineNumber,
        string member)
    {
        _version = version;
        _environment = environment;
        CurrentFor(new ExecutionContext(microservice, Current.Tenant, version, environment, Current.CorrelationId, Current.Claims, Current.CultureInfo), filePath, lineNumber, member);
    }

    /// <inheritdoc/>
    public ExecutionContext System(string filePath, int lineNumber, string member) =>
        CurrentFor(TenantId.System, CorrelationId.System, filePath, lineNumber, member);

    /// <inheritdoc/>
    public ExecutionContext System(CorrelationId correlationId, string filePath, int lineNumber, string member) =>
        CurrentFor(TenantId.System, correlationId, filePath, lineNumber, member);

    /// <inheritdoc/>
    public ExecutionContext CurrentFor(TenantId tenant, string filePath, int lineNumber, string member) =>
        CurrentFor(Current.Microservice, tenant, CorrelationId.New(), Claims.Empty, filePath, lineNumber, member);

    /// <inheritdoc/>
    public ExecutionContext CurrentFor(Microservice microservice, TenantId tenant, string filePath, int lineNumber, string member) =>
        CurrentFor(microservice, tenant, CorrelationId.New(), Claims.Empty, filePath, lineNumber, member);

    /// <inheritdoc/>
    public ExecutionContext CurrentFor(TenantId tenant, CorrelationId correlationId, string filePath, int lineNumber, string member) =>
        CurrentFor(Current.Microservice, tenant, correlationId, Claims.Empty, filePath, lineNumber, member);

    /// <inheritdoc/>
    public ExecutionContext CurrentFor(Microservice microservice, TenantId tenant, CorrelationId correlationId, string filePath, int lineNumber, string member) =>
        CurrentFor(microservice, tenant, correlationId, Claims.Empty, filePath, lineNumber, member);

    /// <inheritdoc/>
    public ExecutionContext CurrentFor(TenantId tenant, CorrelationId correlationId, Claims claims, string filePath, int lineNumber, string member) =>
        CurrentFor(Current.Microservice, tenant, correlationId, claims, filePath, lineNumber, member);

    /// <inheritdoc/>
    public ExecutionContext CurrentFor(Microservice microservice, TenantId tenant, CorrelationId correlationId, Claims claims, string filePath, int lineNumber, string member)
    {
        var executionContext = new ExecutionContext(
            microservice,
            tenant,
            _version,
            _environment,
            correlationId,
            claims,
            CultureInfo.CurrentCulture);

        return CurrentFor(executionContext, filePath, lineNumber, member);
    }

    /// <inheritdoc/>
    public ExecutionContext CurrentFor(
        ExecutionContext context,
        string filePath,
        int lineNumber,
        string member)
    {
        Log.SettingExecutionContext(_logger, context, filePath, lineNumber, member);
        Current = context;
        return context;
    }
}
