// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Booting
{
    /// <summary>
    /// Exception that gets thrown when there are more than one constructors and only one is supported.
    /// </summary>
    public class OnlySingleConstructorSupported : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnlySingleConstructorSupported"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> that has more than one constructor.</param>
        public OnlySingleConstructorSupported(Type type)
            : base($"'{type.AssemblyQualifiedName}' has more than one constructor - only one is supported")
        {
        }
    }
}