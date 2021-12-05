// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.DependencyInversion.Conventions;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Handle convention type {ConventionType}")]
    internal static partial void HandleConvention(ILogger logger, string conventionType);
    
    [LoggerMessage(0, LogLevel.Trace, "Find all binding conventions")]
    internal static partial void FindAllBindingConventions(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Discover and setup bindings")]
    internal static partial void DiscoverAndSetupBindings(ILogger logger);
}
