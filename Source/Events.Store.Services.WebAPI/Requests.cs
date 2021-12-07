// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Security;

using RuntimeExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using RuntimeArtifact = Dolittle.Runtime.Artifacts.Artifact;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

public record ExecutionContext(
    Guid Microservice,
    Guid Tenant,
    Versioning.Version Version,
    string Environment,
    Guid CorrelationId,
    Claim[] Claims)
{
    public RuntimeExecutionContext ToExecutionContext()
        => new(
            Microservice,
            Tenant,
            Version,
            Environment,
            CorrelationId,
            new Claims(Claims),
            CultureInfo.InvariantCulture);
    public static ExecutionContext From(RuntimeExecutionContext executionContext)
        => new(
            executionContext.Microservice,
            executionContext.Tenant,
            executionContext.Version,
            executionContext.Environment,
            executionContext.CorrelationId,
            executionContext.Claims.ToArray());
}

public record CallRequestContext(Guid HeadId, ExecutionContext ExecutionContext);

public record Artifact(Guid Id, uint Generation)
{
    public RuntimeArtifact ToArtifact()
        => new(Id, Generation);

    public static Artifact From(RuntimeArtifact artifact)
        => new(artifact.Id, artifact.Generation);
}

public record JsonRequestUncommittedEvent(string EventSource, Artifact Type, bool Public, string Content)
{
    public UncommittedEvent ToUncommittedEvent()
        => new(EventSource, Type.ToArtifact(), Public, Content);
}

public record JsonRequestUncommittedAggregateEvent(Artifact Type, bool Public, string Content)
{
    public UncommittedEvent ToUncommittedEvent(EventSourceId eventSource)
        => new(eventSource, Type.ToArtifact(), Public, Content);
}

public record JsonRequestUncommittedAggregateEvents(
    string EventSource,
    Guid AggregateRoot,
    ulong AggregateRootVersion,
    JsonRequestUncommittedAggregateEvent[] Events)
{
    public UncommittedAggregateEvents ToUncommittedAggregateEvents()
        => new(
            EventSource,
            new RuntimeArtifact(AggregateRoot, ArtifactGeneration.First),
            AggregateRootVersion,
            new ReadOnlyCollection<UncommittedEvent>(Events.Select(_ => _.ToUncommittedEvent(EventSource)).ToList()));
}

public record CommitRequest(JsonRequestUncommittedEvent[] Events, CallRequestContext CallContext);
public record CommitForAggregateRequest(JsonRequestUncommittedAggregateEvents AggregateEvents, CallRequestContext CallContext);
public record FetchForAggregateRequest(Guid AggregateRoot, string EventSource, CallRequestContext CallContext);