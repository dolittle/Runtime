// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Assemblies;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Dependency model has {RuntimeLibrariesCount} libraries")]
    internal static partial void NumberOfLibraries(ILogger logger, int runtimeLibrariesCount);
    [LoggerMessage(0, LogLevel.Trace, "Dependency model has {LibrariesCount} libraries belonging to an assembly group")]
    internal static partial void NumberOfLibrariesInAssemblyGroup(ILogger logger, int librariesCount);

    [LoggerMessage(0, LogLevel.Trace, "Providing '{LibraryName}, {LibraryVersion}'")]
    internal static partial void ProvidingLibrary(ILogger logger, string libraryName, string libraryVersion);
}
