// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Versioning
{
    /// <summary>
    /// Represents a version number adhering to the SemVer 2.0 standard.
    /// </summary>
    public class Version : Value<Version>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> class.
        /// </summary>
        /// <param name="major">Major version of the software.</param>
        /// <param name="minor">Minor version of the software.</param>
        /// <param name="patch">Path level of the software.</param>
        /// <param name="build">Build number of the software.</param>
        /// <param name="preReleaseString">If prerelease - the prerelease string.</param>
        public Version(
            int major,
            int minor,
            int patch,
            int build,
            string preReleaseString = "")
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Build = build;
            IsPreRelease = !string.IsNullOrEmpty(preReleaseString);
            PreReleaseString = preReleaseString;
        }

        /// <summary>
        /// Gets a <see cref="Version" /> that is not set.
        /// </summary>
        public static Version NotSet => new Version(0, 0, 0, 0);

        /// <summary>
        /// Gets the major version number of the software.
        /// </summary>
        public int Major { get; }

        /// <summary>
        /// Gets the minor version number of the software.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        /// Gets the patch level of the software.
        /// </summary>
        public int Patch { get; }

        /// <summary>
        /// Gets the build number of the software.
        /// </summary>
        public int Build { get; }

        /// <summary>
        /// Gets the prerelease string.
        /// </summary>
        public string PreReleaseString { get; }

        /// <summary>
        /// Gets a value indicating whether or not the software is a prerelease.
        /// </summary>
        public bool IsPreRelease { get; }
    }
}