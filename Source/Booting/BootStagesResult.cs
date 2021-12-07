// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Represents the result from performing the <see cref="BootStages"/>.
/// </summary>
public class BootStagesResult
{
    readonly IDictionary<string, object> _associations;

    /// <summary>
    /// Initializes a new instance of the <see cref="BootStagesResult"/> class.
    /// </summary>
    /// <param name="container">The configured <see cref="IContainer"/>.</param>
    /// <param name="associations"><see cref="IDictionary{TKey, TValue}"/> representing associations done during the different <see cref="BootStage">boot stages</see>.</param>
    /// <param name="bootStageResults"><see cref="BootStageResult">Results</see> from all the <see cref="BootStage">boot stages</see>.</param>
    public BootStagesResult(
        IContainer container,
        IDictionary<string, object> associations,
        IEnumerable<BootStageResult> bootStageResults)
    {
        _associations = associations;
        BootStageResults = bootStageResults;
        Container = container;
    }

    /// <summary>
    /// Gets the results from each <see cref="BootStage"/>.
    /// </summary>
    public IEnumerable<BootStageResult> BootStageResults { get; }

    /// <summary>
    /// Gets the <see cref="IContainer"/>.
    /// </summary>
    public IContainer Container { get; }

    /// <summary>
    /// Get association by key.
    /// </summary>
    /// <param name="key">Key for the association.</param>
    /// <returns>Instance associated.</returns>
    public object GetAssociation(string key)
    {
        if (_associations.ContainsKey(key))
        {
            return _associations[key];
        }
        throw new MissingAssociation(key);
    }
    /// <summary>
    /// Get association by key.
    /// </summary>
    /// <param name="key">Key for the association.</param>
    /// <typeparam name="T">The <see cref="Type"/> if the association.</typeparam>
    /// <returns>Instance associated.</returns>
    public T GetAssociation<T>(string key)
        where T : class
        => GetAssociation(key) as T;
}
