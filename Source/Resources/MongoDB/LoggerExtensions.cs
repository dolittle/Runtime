// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Resources.MongoDB;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static class LoggerExtensions
{
    static readonly Action<ILogger, Guid, Exception> _getResourceCalled = LoggerMessage
        .Define<Guid>(
            LogLevel.Information,
            new EventId(1231142, nameof(GetResourceCalled)),
            "Getting MongoDB resource for {Tenant}");
        
    static readonly Action<ILogger, Guid, Exception> _failedToGetResource = LoggerMessage
        .Define<Guid>(
            LogLevel.Information,
            new EventId(1231143, nameof(FailedToGetResource)),
            "Failed to get MongoDB resource for {Tenant}");

    internal static void GetResourceCalled(this ILogger logger, TenantId tenantId)
        => _getResourceCalled(logger, tenantId, null);

    internal static void FailedToGetResource(this ILogger logger, TenantId tenantId, Exception ex)
        => _failedToGetResource(logger, tenantId, ex);
}
