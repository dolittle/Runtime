// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Log messages for <see cref="Dolittle.Runtime.Tenancy"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Information, "Getting all tenants")]
    internal static partial void GetAllCalled(this ILogger logger);
        
    [LoggerMessage(0, LogLevel.Warning, "An error occurred while getting all tenants")]
    internal static partial void FailedToGetAll(this ILogger logger, Exception ex);
}
