// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Client;

/// <summary>
/// Defines a system that knows about <see cref="BuildResults"/> for the different heads that has connected to the Runtime.
/// </summary>
public interface IBuildResultsForHeads
{
    /// <summary>
    /// Gets build results for the <see cref="HeadId"/>
    /// </summary>
    /// <param name="head">The <see cref="HeadId"/>.</param>
    public BuildResults GetFor(HeadId head);

    /// <summary>
    /// Adds build results for the <see cref="HeadId"/>
    /// </summary>
    /// <param name="head">The <see cref="HeadId"/>.</param>
    /// <param name="results">The <see cref="BuildResults"/>.</param>
    public void AddFor(HeadId head, BuildResults results);
}
