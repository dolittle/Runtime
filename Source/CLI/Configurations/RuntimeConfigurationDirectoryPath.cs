// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.Rudimentary;
namespace CLI.Configurations
{
    /// <summary>
    /// Represents the path to the Dolittle Runtime configuration directory.
    /// </summary>
    public record RuntimeConfigurationDirectoryPath(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicitly convert from a <see cref="string"/> to an <see cref="RuntimeConfigurationDirectoryPath"/>.
        /// </summary>
        /// <param name="path">RuntimeConfigurationDirectoryPath as <see cref="string"/>.</param>
        public static implicit operator RuntimeConfigurationDirectoryPath(string path) => new(path);
    }
}