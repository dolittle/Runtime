// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Client;

/// <summary>
/// Extension methods for build result.
/// </summary>
public static class BuildResultExtensions
{
    public static BuildResult ToBuildResult(this Contracts.BuildResult result)
        => new(
            result.Type switch
            {
                Contracts.BuildResult.Types.Type.Information => BuildResultType.Information,
                Contracts.BuildResult.Types.Type.Failure => BuildResultType.Failure,
                Contracts.BuildResult.Types.Type.Error => BuildResultType.Error,
                _ => BuildResultType.Information
            },
            result.Message);
    
    public static ArtifactBuildResult ToArtifactBuildResult(this Contracts.ArtifactBuildResult result)
        => new(
            result.Aritfact.ToArtifact(),
            result.Alias,
            result.BuildResult.ToBuildResult());

    public static BuildResults ToBuildResults(this Contracts.BuildResults results)
        => new(
            results.EventTypes.Select(_ => _.ToArtifactBuildResult()),
            results.AggregateRoots.Select(_ => _.ToArtifactBuildResult()),
            results.EventHandlers.Select(_ => _.ToArtifactBuildResult()),
            results.Projections.Select(_ => _.ToArtifactBuildResult()),
            results.Embeddings.Select(_ => _.ToArtifactBuildResult()),
            results.Filters.Select(_ => _.ToArtifactBuildResult()),
            results.Other.Select(_ => _.ToBuildResult()));
    
    public static Contracts.BuildResult ToProtobuf(this BuildResult result) => new()
    {
        Message = result.Message,
        Type = result.Type switch
        {
            BuildResultType.Information => Contracts.BuildResult.Types.Type.Information,
            BuildResultType.Failure => Contracts.BuildResult.Types.Type.Failure,
            BuildResultType.Error => Contracts.BuildResult.Types.Type.Error,
            _ => Contracts.BuildResult.Types.Type.Information
        }
    };
    
    public static Contracts.ArtifactBuildResult ToProtobuf(this ArtifactBuildResult result) => new()
    {
        Alias = result.Alias,
        Aritfact = result.Artifact.ToProtobuf(),
        BuildResult = result.Result.ToProtobuf()
    };

    public static Contracts.BuildResults ToProtobuf(this BuildResults results)
        => new ()
        {
            Embeddings = { results.Embeddings.Select(_ => _.ToProtobuf()) },
            Filters = { results.Filters.Select(_ => _.ToProtobuf()) },
            Other = { results.Other.Select(_ => _.ToProtobuf()) },
            Projections = { results.Projections.Select(_ => _.ToProtobuf()) },
            AggregateRoots = { results.AggregateRoots.Select(_ => _.ToProtobuf()) },
            EventHandlers = { results.EventHandlers.Select(_ => _.ToProtobuf()) },
            EventTypes = { results.EventTypes.Select(_ => _.ToProtobuf()) }
        };
}
