// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Configuration;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Configuration Object provider : {ConfigurationObjectProviderType}")]
    internal static partial void ConfigurationObjectProvider(ILogger logger, string configurationObjectProviderType);
    
    [LoggerMessage(0, LogLevel.Trace, "Try to provide '{ConfigurationObjectName} - {ConfigurationObjectType}'")]
    internal static partial void TryProvide(ILogger logger, string configurationObjectName, string configurationObjectType);
    
    [LoggerMessage(0, LogLevel.Trace, "Provide '{ConfigurationObjectName} - {ConfigurationObjectType}' using {ConfigurationObjectProviderType}")]
    internal static partial void ProvideConfiguration(ILogger logger, string configurationObjectName, string configurationObjectType, string configurationObjectProviderType);

    [LoggerMessage(0, LogLevel.Trace, "Ask '{ConfigurationObjectProviderType}' if it can provide the configuration type '{ConfigurationObjectName} - {ConfigurationObjectTypeName}'")]
    internal static partial void CanProviderProvideConfigurationType(ILogger logger, string configurationObjectProviderType, string configurationObjectName, string configurationObjectTypeName);
}
