// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Autofac;
using Autofac.Core.Resolving.Pipeline;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.DependencyInversion.Logging;

/// <summary>
/// Represents an implementation of <see cref="IResolveMiddleware"/> that resolves untyped <see cref="ILogger"/> for registered types.
/// </summary>
public class LoggerResolvingMiddleware : IResolveMiddleware
{
    readonly Type _serviceType;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerResolvingMiddleware"/> class.
    /// </summary>
    /// <param name="serviceType"></param>
    public LoggerResolvingMiddleware(Type serviceType)
    {
        _serviceType = serviceType;
    }

    /// <inheritdoc />
    public PipelinePhase Phase => PipelinePhase.ParameterSelection;

    /// <inheritdoc />
    public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next)
    {
        var factory = context.Resolve<ILoggerFactory>();
        var logger = factory.CreateLogger(_serviceType);
        
        context.ChangeParameters(context.Parameters.Append( TypedParameter.From(logger) ));
        next(context);
    }
}
