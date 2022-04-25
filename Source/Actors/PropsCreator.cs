using System;
using System.Linq;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Actors;

/// <summary>
/// Represents an implementation of <see cref="ICreateProps"/>.
/// </summary>
[Singleton, PerTenant]
public class CreateProps : ICreateProps
{
    readonly IServiceProvider _serviceProvider;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProps"/> class.
    /// </summary>
    /// <param name="serviceProvider">The tenant specific service provider.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public CreateProps(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public Props PropsFor<TActor>(params object[] parameters) where TActor : IActor
        => Props.FromProducer(() => ActivatorUtilities.CreateInstance<TActor>(_serviceProvider, parameters.Append(_loggerFactory.CreateLogger<TActor>())));
}
