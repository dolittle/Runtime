// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.DependencyInversion.Booting;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "DependencyInversion start")]
    internal static partial void StartDependencyInversion(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Discover and Build bindings")]
    internal static partial void DiscoverAndBuildBindings(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Discover and setup bindings")]
    internal static partial void DiscoverAndSetupBindings(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Discover container")]
    internal static partial void DiscoverContainer(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Return boot result")]
    internal static partial void ReturnBootResult(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Discover bindings")]
    internal static partial void DiscoverBindings(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Create a new binding collection")]
    internal static partial void CreateNewBindingCollection(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Discovered Binding : {BindingServiceName} - {BindingStrategyTypeName}")]
    internal static partial void DiscoveredBinding(ILogger logger, string bindingServiceName, string bindingStrategyTypeName);
    
    [LoggerMessage(0, LogLevel.Trace, "Discover binding providers and get bindings")]
    internal static partial void DiscoverBindingProviders(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Compose bindings in new collection")]
    internal static partial void ComposeBindings(ILogger logger);
    
    
}
