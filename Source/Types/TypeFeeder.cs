// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Threading.Tasks;
using Dolittle.Runtime.Assemblies;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Types;

/// <summary>
/// Represents an implementation of <see cref="ITypeFinder"/>.
/// </summary>
public class TypeFeeder : ITypeFeeder
{
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeFeeder"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger"/> used for logging.</param>
    public TypeFeeder(ILogger logger) => _logger = logger;

    /// <inheritdoc/>
    public void Feed(IAssemblies assemblies, IContractToImplementorsMap map)
        => Parallel.ForEach(
            assemblies.GetAll(),
            assembly => FeedWithTypesFromAssembly(assembly, map));

    void FeedWithTypesFromAssembly(Assembly assembly, IContractToImplementorsMap map)
    {
        try
        {
            map.Feed(assembly.GetTypes());
        }
        catch (ReflectionTypeLoadException ex)
        {
            foreach (var loaderException in ex.LoaderExceptions)
                _logger.LogError("TypeFeed failure for assembly {AssemblyName} : {LoaderExceptionSource} {LoaderExceptionMessage}", assembly.FullName, loaderException.Source, loaderException.Message);
        }
    }
}