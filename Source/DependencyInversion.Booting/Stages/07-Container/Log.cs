// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.DependencyInversion.Booting.Stages;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Starting DependencyInversion with predefined container type '{ContainerType}'")]
    internal static partial void StartingDependencyInversion(ILogger logger, string containerType);
    
    [LoggerMessage(0, LogLevel.Trace, "Using container of type '{ContainerType}'")]
    internal static partial void UsingContainerOfType(ILogger logger, string containerType);
}
