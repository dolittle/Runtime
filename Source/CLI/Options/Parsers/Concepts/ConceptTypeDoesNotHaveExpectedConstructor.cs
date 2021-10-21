// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.CLI.Options.Parsers.Concepts
{
    /// <summary>
    /// Exception that gets thrown when trying to initialize an implementation of <see cref="ConceptAs{TValue}"/> of type <see cref="Guid"/> that does not have the expected constructor.
    /// </summary>
    public class ConceptTypeDoesNotHaveExpectedConstructor : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptTypeDoesNotHaveExpectedConstructor"/> class.
        /// </summary>
        /// <param name="concept">The type of the concept.</param>
        /// <param name="value">The type of the concept value.</param>
        public ConceptTypeDoesNotHaveExpectedConstructor(Type concept, Type value)
            : base($"The type {concept.Name} does not have a constructor that accepts a {value.Name} as the only argument.")
        {
        }
    }
}
