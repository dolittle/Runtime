// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    static class LoggerExtensions
    {
        static readonly Action<ILogger, Exception> _failure = LoggerMessage
            .Define(
                LogLevel.Warning,
                new EventId(1231141, nameof(Failure)),
                "An error occurred");
        
        static readonly Action<ILogger, Exception> _getAll = LoggerMessage
            .Define(
                LogLevel.Information,
                new EventId(242121231, nameof(GetAll)),
                "Getting all Tenants");
        
        internal static void Failure(this ILogger logger, Exception ex)
            => _failure(logger, ex);

        internal static void GetAll(this ILogger logger)
            => _getAll(logger, null);
    }
}
