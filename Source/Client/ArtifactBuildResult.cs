// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Client;

public record ArtifactBuildResult(Artifact Artifact, string Alias, BuildResult Result)
{
    public void Log(ILogger logger, HeadId head, string artifactType)
    {
        if (!logger.IsEnabled(Result.LogLevel))
        {
            return;
        }
        if (string.IsNullOrEmpty(Alias))
        {
            logger.Log(Result.LogLevel, "{ArtifactType} {Artifact}\n\t{Message}", artifactType, Artifact, Result.Message);
        }
        else
        {
            logger.Log(Result.LogLevel, "{ArtifactType} {Alias} {Artifact}\n\t{Message}", artifactType, Alias, Artifact, Result.Message);
        }
    }
}
