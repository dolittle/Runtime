// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Extension methods for <see cref="Version" />.
/// </summary>
public static class VersionExtension
{
    /// <summary>
    /// Converts <see cref="Version" /> to <see cref="Domain.Platform.Version" />.
    /// </summary>
    /// <param name="version"><see cref="Version" />.</param>
    /// <returns>Converted <see cref="Domain.Platform.Version" />.</returns>
    public static Domain.Platform.Version ToVersion(this Version version) =>
        new(version.Major, version.Minor, version.Patch, version.Build, version.PreRelease);

    /// <summary>
    /// Converts <see cref="Domain.Platform.Version" /> to <see cref="Version" />.
    /// </summary>
    /// <param name="version"><see cref="Domain.Platform.Version" />.</param>
    /// <returns>Converted <see cref="Version" />.</returns>
    public static Version ToStoreRepresentation(this Domain.Platform.Version version) =>
        new(version.Major, version.Minor, version.Patch, version.Build, version.PreReleaseString);
}
