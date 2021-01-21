// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text.RegularExpressions;

namespace Dolittle.Runtime.Versioning
{
    /// <summary>
    /// Represents an implementation of <see cref="IVersionConverter"/>.
    /// </summary>
    public class VersionConverter : IVersionConverter
    {
        static readonly Regex _versionRegex = new Regex("(\\d+).(\\d+).(\\d+)-*([\\w]+)*[+-.]*(\\d+)*", RegexOptions.Compiled);

        /// <inheritdoc/>
        public Version FromString(string versionAsString)
        {
            var result = _versionRegex.Match(versionAsString);
            if (!result.Success) throw new InvalidVersionString(versionAsString);
            var major = int.Parse(result.Groups[1].Value, CultureInfo.InvariantCulture);
            var minor = int.Parse(result.Groups[2].Value, CultureInfo.InvariantCulture);
            var patch = int.Parse(result.Groups[3].Value, CultureInfo.InvariantCulture);
            var buildGroup = result.Groups[5].Value?.Length == 0 ? 4 : 5;
            int.TryParse(result.Groups[buildGroup].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int build);
            var isRelease = result.Groups[4].Value?.Length == 0;

            if (!isRelease)
                return new Version(major, minor, patch, build, result.Groups[4].Value);
            else
                return new Version(major, minor, patch, build);
        }

        /// <inheritdoc/>
        public string ToString(Version version)
        {
            var postfix = version.IsPreRelease ? $"-{version.PreReleaseString}.{version.Build}" : string.Empty;
            return $"{version.Major}.{version.Minor}.{version.Patch}{postfix}";
        }
    }
}