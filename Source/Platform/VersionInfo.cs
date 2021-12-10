// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Platform;

/// <summary>
/// Represents a container for the Dolittle Runtime version.
/// </summary>
public static class VersionInfo
{
    /// <summary>
    /// Gets the current <see cref="Version"/> of the Dolittle Runtime.
    /// </summary>
    public static Version CurrentVersion => new(377, 389, 368, 0, "PRERELEASE");
}
