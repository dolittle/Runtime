using System;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Exception that gets thrown when an open generic interface has more that one generic parameter when it's expected to have only one.
/// </summary>
public class OpenGenericInterfaceShouldHaveOnlyOneGenericParameter : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericInterfaceShouldHaveOnlyOneGenericParameter"/> class.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="genericInterface">The generic interface with multiple generic parameters.</param>
    public OpenGenericInterfaceShouldHaveOnlyOneGenericParameter(Type type, Type genericInterface)
        : base($"{type} implements open generic interface {genericInterface} that has more than one generic parameters")
    {
    }
}