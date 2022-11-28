// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Client;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class LoggerExtensions
{
    [LoggerMessage(0, LogLevel.Information, "Build results has been added for Head {Head}")]
    internal static partial void BuildResultsAddedForHead(this ILogger logger, HeadId head);
}
