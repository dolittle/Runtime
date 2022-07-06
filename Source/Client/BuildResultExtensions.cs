// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Google.Protobuf.Collections;

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

    public static BuildResults ToBuildResults(this RepeatedField<Contracts.BuildResult> results)
        => new(results.Select(_ => ToBuildResult(_)).ToList());
    
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

    public static RepeatedField<Contracts.BuildResult> ToProtobuf(this BuildResults results)
    {
        var pbResults = new RepeatedField<Contracts.BuildResult>();
        pbResults.AddRange(results.Select(_ => _.ToProtobuf()));
        return pbResults;
    }
}
