// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.Runtime.Execution
{
    /// <summary>
    /// Exception that gets thrown when the signature of a method does not match how it is called.
    /// Typically used when dynamically invoking a <see cref="WeakDelegate"/>.
    /// </summary>
    public class InvalidMethodSignature : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMethodSignature"/> class.
        /// </summary>
        /// <param name="expectedSignature"><see cref="MethodInfo"/> that represents the expected signature.</param>
        public InvalidMethodSignature(MethodInfo expectedSignature)
            : base($"Method '{expectedSignature.Name}' was invoked with the wrong signature, expected: {expectedSignature}")
        {
        }
    }
}
