// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Represents the concept of a runtime environment - e.g. Testing, Development, Staging, Production.
    /// </summary>
    public class Environment : ConceptAs<string>
    {
        /// <summary>
        /// Represents an undetermined environment.
        /// </summary>
        public static readonly Environment Undetermined = "Undetermined";

        /// <summary>
        /// Represents a production like environment.
        /// </summary>
        public static readonly Environment Production = "Production";

        /// <summary>
        /// Represents a development like environmen.
        /// </summary>
        public static readonly Environment Development = "Development";

        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="Environment"/>.
        /// </summary>
        /// <param name="environment">The environment string.</param>
        public static implicit operator Environment(string environment)
        {
            return new Environment { Value = environment };
        }
    }
}
