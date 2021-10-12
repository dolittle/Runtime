// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.Rudimentary;
namespace Dolittle.Runtime.CLI.Configurations
{
    /// <summary>
    /// Represents the name of a Dolittle Runtime configuration file.
    /// </summary>
    public record RuntimeConfigurationName(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicitly convert from a <see cref="string"/> to an <see cref="RuntimeConfigurationName"/>.
        /// </summary>
        /// <param name="name">RuntimeConfigurationName as <see cref="string"/>.</param>
        public static implicit operator RuntimeConfigurationName(string name) => new(name);
    }
}