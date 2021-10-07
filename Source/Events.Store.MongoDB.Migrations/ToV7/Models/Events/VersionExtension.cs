// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7.Models.Events
{
    /// <summary>
    /// Extension methods for <see cref="Version" />.
    /// </summary>
    public static class VersionExtension
    {
        /// <summary>
        /// Converts <see cref="Version" /> to <see cref="Versioning.Version" />.
        /// </summary>
        /// <param name="version"><see cref="Version" />.</param>
        /// <returns>Converted <see cref="Versioning.Version" />.</returns>
        public static Versioning.Version ToVersion(this Version version) =>
            new Versioning.Version(version.Major, version.Minor, version.Patch, version.Build, version.PreRelease);

        /// <summary>
        /// Converts <see cref="Versioning.Version" /> to <see cref="Version" />.
        /// </summary>
        /// <param name="version"><see cref="Versioning.Version" />.</param>
        /// <returns>Converted <see cref="Version" />.</returns>
        public static Version ToStoreRepresentation(this Versioning.Version version) =>
            new Version(version.Major, version.Minor, version.Patch, version.Build, version.PreReleaseString);
    }
}
