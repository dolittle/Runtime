// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Versioning
{
    /// <summary>
    /// Represents a version number adhering to the SemVer 2.0 standard.
    /// </summary>
    public record Version(int Major, int Minor, int Patch, int Build = 0, string PreReleaseString = "")
    {
        /// <summary>
        /// Gets a <see cref="Version" /> that is not set.
        /// </summary>
        public static Version NotSet => new(0, 0, 0, 0);

        /// <summary>
        /// Gets a value indicating whether or not the software is a prerelease.
        /// </summary>
        public bool IsPreRelease => !string.IsNullOrEmpty(PreReleaseString);

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = $"{Major}.${Minor}.${Patch}.${Build}";
            if (IsPreRelease) result += $"-{PreReleaseString}.{Build}";
            return result;
        }
    }
}