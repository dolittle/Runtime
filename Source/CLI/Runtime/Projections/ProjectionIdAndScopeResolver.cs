// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// Represents an implementation of <see cref="IResolveProjectionIdAndScope"/>.
/// </summary>
public class ProjectionIdAndScopeResolver : IResolveProjectionIdAndScope
{
    readonly IManagementClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionIdAndScopeResolver"/> class.
    /// </summary>
    /// <param name="client">The management client to use to get all registered projections.</param>
    public ProjectionIdAndScopeResolver(IManagementClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public async Task<Try<ProjectionIdentifierAndScope>> Resolve(MicroserviceAddress runtime, string identifierOrAlias, ScopeId scope = null)
    {
        var projections = await _client.GetAll(runtime).ConfigureAwait(false);

        var matchesWithoutScope = projections.Where(projection =>
            projection.HasAlias
                ? projection.Alias.Value == identifierOrAlias
                : projection.Id.ToString() == identifierOrAlias
        ).ToList();

        if (matchesWithoutScope.Count == 1)
        {
            return new ProjectionIdentifierAndScope(matchesWithoutScope[0].Id, matchesWithoutScope[0].Scope);
        }

        scope ??= ScopeId.Default;
        var matchesWithScope = matchesWithoutScope.Where(projection => projection.Scope == scope).ToList();

        if (matchesWithScope.Count == 1)
        {
            return new ProjectionIdentifierAndScope(matchesWithScope[0].Id, matchesWithScope[0].Scope);
        }

        if (matchesWithoutScope.Count > 1)
        {
            return new MultipleProjectionsWithIdentifierOrAliasInScope(identifierOrAlias, scope, matchesWithoutScope.Count);
        }

        return new NoProjectionWithIdentifierOrAliasInScope(identifierOrAlias, scope);
    }
}
