// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Validation
{
    /// <summary>
    /// Exception that gets thrown when a value coming in is of the wrong type from what is expected in a rule.
    /// </summary>
    public class ValueTypeMismatch : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypeMismatch"/> class.
        /// </summary>
        /// <param name="expected">Expected type for value.</param>
        /// <param name="actual">Actual type for value.</param>
        public ValueTypeMismatch(Type expected, Type actual)
            : base($"Expected '{expected.Name}' but got '{actual.Name}'")
        {
        }
    }
}
