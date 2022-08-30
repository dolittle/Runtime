// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Client;

/// <summary>
/// Represents an implementation of <see cref="IBuildResultsForHeads"/>.
/// </summary>
[Singleton]
public class BuildResultsForHeads : IBuildResultsForHeads
{
    readonly ILogger _logger;
    BuildResults? _buildResults;

    public BuildResultsForHeads(ILogger logger) => _logger = logger;

    /// <inheritdoc />
    public BuildResults GetFor(HeadId head)
        => _buildResults ?? BuildResults.Empty;

    /// <inheritdoc />
    public void AddFor(HeadId head, BuildResults results)
    {
        _buildResults = results;
        _logger.BuildResultsAddedForHead(head);
        results.Log(_logger, head);
    }
}
