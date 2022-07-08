// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.Hosting.Endpoints;

/// <summary>
/// Log messages for <see cref="Dolittle.Runtime.Services.Hosting.Endpoints"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Mapping discovered gRPC service {Implementation} to endpoint {Visibility}")]
    internal static partial void MappingDiscoveredGrpcService(this ILogger logger, Type implementation, EndpointVisibility visibility);
}
