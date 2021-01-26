// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Versioning;
using VersionContract = Dolittle.Versioning.Contracts.Version;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Represents conversion extensions for the common execution types.
    /// </summary>
    public static class VersioningExtensions
    {
        /// <summary>
        /// Convert a <see cref="Version"/> to <see cref="VersionContract"/>.
        /// </summary>
        /// <param name="version"><see cref="Version"/> to convert from.</param>
        /// <returns>Converted <see cref="VersionContract"/>.</returns>
        public static VersionContract ToProtobuf(this Version version) =>
            new()
                {
                    Major = version.Major,
                    Minor = version.Minor,
                    Patch = version.Patch,
                    Build = version.Build,
                    PreReleaseString = version.PreReleaseString
                };

        /// <summary>
        /// Convert a <see cref="VersionContract"/> to <see cref="Version"/>.
        /// </summary>
        /// <param name="version"><see cref="VersionContract"/> to convert from.</param>
        /// <returns>Converted <see cref="Version"/>.</returns>
        public static Version ToVersion(this VersionContract version) =>
            new(version.Major, version.Minor, version.Patch, version.Build, version.PreReleaseString);
    }
}