// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Client;

/// <summary>
/// Represents a <see cref="ReadOnlyCollection{T}"/> of <see cref="BuildResult"/>.
/// </summary>
public class BuildResults : ReadOnlyCollection<BuildResult>
{
    public BuildResults(IList<BuildResult> list)
        : base(list)
    { }
}
