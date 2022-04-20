using System;
using System.Linq;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Actors;

[Singleton, PerTenant]
public class CreateProps : ICreateProps
{
    readonly IServiceProvider _serviceProvider;
    readonly ILoggerFactory _loggerFactory;

    public CreateProps(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public Props PropsFor<TActor>(params object[] parameters) where TActor : IActor
        => Props.FromProducer(() => ActivatorUtilities.CreateInstance<TActor>(_serviceProvider, parameters.Append(_loggerFactory.CreateLogger<TActor>())));
}
