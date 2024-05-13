// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Events.Processing.Contracts.Projections;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents the implementation of <see cref="ProjectionsBase"/>.
/// </summary>
[PrivateService]
public class ProjectionsService : ProjectionsBase
{
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsService"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    public ProjectionsService(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public override Task Connect(
        IAsyncStreamReader<ProjectionClientToRuntimeMessage> runtimeStream,
        IServerStreamWriter<ProjectionRuntimeToClientMessage> clientStream,
        ServerCallContext context)
    {
        _logger.ConnectingUnsupportedProjections();

        throw new UnsupportedClientVersion("Migrate to SDK version 23 to use Projections.");
    }
}
