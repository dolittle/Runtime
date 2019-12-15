// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Validation.MetaData
{
    /// <summary>
    /// Represents a system for retrieving validation metadata.
    /// </summary>
    public interface IValidationMetaData
    {
        /// <summary>
        /// Get metadata for a specific type.
        /// </summary>
        /// <param name="typeForValidation">The <see cref="Type"/> that will be validated.</param>
        /// <returns>The actual metadata.</returns>
        TypeMetaData GetMetaDataFor(Type typeForValidation);
    }
}
