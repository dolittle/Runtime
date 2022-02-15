// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Booting.Stages;

/// <summary>
/// Represents the <see cref="BootStage.Logging"/> stage of booting.
/// </summary>
public class Logging : ICanPerformBootStage<LoggingSettings>
{
    /// <inheritdoc/>
    public BootStage BootStage => BootStage.Logging;

    /// <inheritdoc/>
    public void Perform(LoggingSettings settings, IBootStageBuilder builder)
    {
        var loggerFactory = settings.LoggerFactory;

        builder.Associate(WellKnownAssociations.LoggerFactory, loggerFactory);
        builder.Bindings.Bind<ILoggerFactory>().To(loggerFactory);
        builder.Bindings.Bind(typeof(ILogger<>)).To(context =>
        {

            var createLoggerMethodNonGeneric = GetType().GetMethod(nameof(CreateLogger), BindingFlags.Static | BindingFlags.NonPublic);
            var createLoggerMethod = createLoggerMethodNonGeneric.MakeGenericMethod(context.Service.GetGenericArguments()[0]);
            var logger = createLoggerMethod.Invoke(this, new object[] { loggerFactory });
            return logger;
        });
        builder.Bindings.Bind<ILogger>().To(() => loggerFactory.CreateLogger("UnknownMessageSource"));

        var logger = loggerFactory.CreateLogger<Logging>();
        Log.BootStageLogging(logger);

        var executionContextLogger = loggerFactory.CreateLogger<ExecutionContextManager>();

        ExecutionContextManager.SetInitialExecutionContext(executionContextLogger);

        builder.Bindings.Bind<IExecutionContextManager>().To(new ExecutionContextManager(executionContextLogger));
    }

    static ILogger<T> CreateLogger<T>(ILoggerFactory factory) => new Logger<T>(factory);
}
