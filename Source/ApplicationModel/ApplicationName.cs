// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.ApplicationModel
{
    /// <summary>
    /// Represents the name of <see cref="Application"/>.
    /// </summary>
    public class ApplicationName : ConceptAs<string>
    {
        /// <summary>
        /// Gets the <see cref="ApplicationName"/> representing an undefined name.
        /// </summary>
        public static readonly ApplicationName NotSet = "[Not Set]";

        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to a <see cref="ApplicationName"/>.
        /// </summary>
        /// <param name="applicationName">Name of the <see cref="ApplicationName"/>.</param>
        public static implicit operator ApplicationName(string applicationName)
        {
            return new ApplicationName { Value = applicationName };
        }
    }
}