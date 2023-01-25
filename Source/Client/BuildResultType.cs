// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Client;

/// <summary>
/// Represents the type of a <see cref="BuildResult"/>.
/// </summary>
public enum BuildResultType
{
    /// <summary>
    /// Informational build result.
    /// </summary>
    Information,
    
    /// <summary>
    /// Non critical failure build result.
    /// </summary>
    Failure,
    
    /// <summary>
    /// Critical failure build result.
    /// </summary>
    Error
}
