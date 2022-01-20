// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Tenancy;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static class LoggerExtensions
{
    static readonly Action<ILogger, Exception> _getAllCalled = LoggerMessage
        .Define(
            LogLevel.Information,
            new EventId(242121231, nameof(GetAllCalled)),
            "Getting all tenants");
        
    static readonly Action<ILogger, Exception> _failedToGetAll = LoggerMessage
        .Define(
            LogLevel.Warning,
            new EventId(1231141, nameof(FailedToGetAll)),
            "An error occured while getting all tenants");

    internal static void GetAllCalled(this ILogger logger)
        => _getAllCalled(logger, null);
        
    internal static void FailedToGetAll(this ILogger logger, Exception ex)
        => _failedToGetAll(logger, ex);
}