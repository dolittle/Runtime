// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Server.Handshake;

static partial class Log
{
    [LoggerMessage(
        0,
        LogLevel.Warning,
        "Cannot perform handshake between Head and Runtime because the Head's version of contracts ({HeadContractsVersion}) is incompatible with the Runtime version of contracts ({RuntimeContractsVersion})")]
    internal static partial void HeadAndRuntimeContractsIncompatible(ILogger logger, Versioning.Version headContractsVersion, Versioning.Version runtimeContractsVersion);

    [LoggerMessage(0, LogLevel.Warning, "An error occurred while performing handshake")]
    internal static partial void ErrorWhilePerformingHandshake(ILogger logger, Exception ex);
}
