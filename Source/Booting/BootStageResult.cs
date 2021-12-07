// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Booting;

/// <summary>
/// Represents the result of a <see cref="BootStage"/>.
/// </summary>
public class BootStageResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BootStageResult"/> class.
    /// </summary>
    /// <param name="container"><see cref="IContainer"/> to use.</param>
    /// <param name="bindings"><see cref="IBindingCollection"/> containing all bindings for the <see cref="BootStage"/>.</param>
    /// <param name="associations">Associations created during <see cref="BootStage"/>.</param>
    public BootStageResult(
        IContainer container,
        IBindingCollection bindings,
        IDictionary<string, object> associations)
    {
        Container = container;
        Bindings = bindings;
        Associations = new ReadOnlyDictionary<string, object>(associations);
    }

    /// <summary>
    /// Gets the Container to use from the <see cref="BootStage"/> and on.
    /// </summary>
    public IContainer Container { get; }

    /// <summary>
    /// Gets the <see cref="IBindingCollection">bindings</see> built from the stage.
    /// </summary>
    public IBindingCollection Bindings { get; }

    /// <summary>
    /// Gets any associations of type vs instance.
    /// </summary>
    public IReadOnlyDictionary<string, object> Associations { get; }
}