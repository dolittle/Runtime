// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents a version.
    /// </summary>
    public class Version
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> class.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch version number.</param>
        /// <param name="build">The build number.</param>
        /// <param name="preRelease">The pre-release string.</param>
        public Version(int major, int minor, int patch, int build, string preRelease)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Build = build;
            PreRelease = preRelease;
        }

        /// <summary>
        /// Gets or sets the major version number.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Gets or sets the minor version number.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Gets or sets the patch version number.
        /// </summary>
        public int Patch { get; set; }

        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        public int Build { get; set; }

        /// <summary>
        /// Gets or sets the pre-release string.
        /// </summary>
        public string PreRelease { get; set; }
    }
}
