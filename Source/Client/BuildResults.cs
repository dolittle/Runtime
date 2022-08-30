// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Client;

/// <summary>
/// Represents a <see cref="ReadOnlyCollection{T}"/> of <see cref="BuildResult"/>.
/// </summary>
public class BuildResults
{
    public static BuildResults Empty => new(
        Enumerable.Empty<ArtifactBuildResult>(),
        Enumerable.Empty<ArtifactBuildResult>(),
        Enumerable.Empty<ArtifactBuildResult>(),
        Enumerable.Empty<ArtifactBuildResult>(),
        Enumerable.Empty<ArtifactBuildResult>(),
        Enumerable.Empty<ArtifactBuildResult>(),
        Enumerable.Empty<BuildResult>());

    public BuildResults(
        IEnumerable<ArtifactBuildResult> eventTypes,
        IEnumerable<ArtifactBuildResult> aggregateRoots,
        IEnumerable<ArtifactBuildResult> eventHandlers,
        IEnumerable<ArtifactBuildResult> projections,
        IEnumerable<ArtifactBuildResult> embeddings,
        IEnumerable<ArtifactBuildResult> filters,
        IEnumerable<BuildResult> other)
    {
        EventTypes = eventTypes;
        AggregateRoots = aggregateRoots;
        EventHandlers = eventHandlers;
        Projections = projections;
        Embeddings = embeddings;
        Filters = filters;
        Other = other;
    }

    /// <summary>
    /// The <see cref="IEnumerable{T}"/> of <see cref="ArtifactBuildResult"/> for event types.
    /// </summary>
    public IEnumerable<ArtifactBuildResult> EventTypes { get; }
    
    /// <summary> 
    /// The <see cref="IEnumerable{T}"/> of <see cref="ArtifactBuildResult"/> for aggregate roots.
    /// </summary>
    public IEnumerable<ArtifactBuildResult> AggregateRoots { get; }
    
    /// <summary> 
    /// The <see cref="IEnumerable{T}"/> of <see cref="ArtifactBuildResult"/> for event handlers.
    /// </summary>
    public IEnumerable<ArtifactBuildResult> EventHandlers { get; }
    
    /// <summary> 
    /// The <see cref="IEnumerable{T}"/> of <see cref="ArtifactBuildResult"/> for projections.
    /// </summary>
    public IEnumerable<ArtifactBuildResult> Projections { get; }
    
    /// <summary> 
    /// The <see cref="IEnumerable{T}"/> of <see cref="ArtifactBuildResult"/> for embeddings.
    /// </summary>
    public IEnumerable<ArtifactBuildResult> Embeddings { get; }
    
    /// <summary> 
    /// The <see cref="IEnumerable{T}"/> of <see cref="ArtifactBuildResult"/> for filters.
    /// </summary>
    public IEnumerable<ArtifactBuildResult> Filters { get; }
    
    /// <summary> 
    /// The <see cref="IEnumerable{T}"/> of <see cref="BuildResult"/> for other build results.
    /// </summary>
    public IEnumerable<BuildResult> Other { get; }

    public void Log(ILogger logger, HeadId headId)
    {
        foreach (var result in Other)
        {
            result.Log(logger, headId);
        }
        Log(logger, headId, EventTypes, "Event Type");
        Log(logger, headId, AggregateRoots, "Aggregate Root Type");
        Log(logger, headId, EventHandlers, "Event Handler");
        Log(logger, headId, Projections, "Projection");
        Log(logger, headId, Embeddings, "Embedding");
        Log(logger, headId, Embeddings, "Filters");
    }

    static void Log(ILogger logger, HeadId headId, IEnumerable<ArtifactBuildResult> results, string artifactType)
    {
        foreach (var result in results)
        {
            result.Log(logger, headId, artifactType);
        }
    }
}
