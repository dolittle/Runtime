// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Configuration;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Trace, "Bind configuration object '{ConfigurationObjectName} - {ConfigurationObjectType}'")]
    internal static partial void BindConfigurationObject(ILogger logger, string configurationObjectName, string configurationObjectType);
    
    [LoggerMessage(0, LogLevel.Trace, "Providing configuration object '{ConfigurationObjectName} - {ConfigurationTypeName}' - {ConfigurationObjectHash}")]
    internal static partial void ProvidingConfigurationObject(ILogger logger, string configurationObjectName, string configurationTypeName, int configurationObjectHash);
}
