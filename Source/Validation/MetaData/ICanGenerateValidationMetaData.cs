// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Validation.MetaData
{
    /// <summary>
    /// Defines the generator that generates metadata for a validator.
    /// </summary>
    public interface ICanGenerateValidationMetaData
    {
        /// <summary>
        /// Generate metadata from a specific type that can be validated.
        /// </summary>
        /// <param name="typeForValidation">The <see cref="Type"/> that will be validated.</param>
        /// <returns>The actual metadata.</returns>
        TypeMetaData GenerateFor(Type typeForValidation);
    }
}
