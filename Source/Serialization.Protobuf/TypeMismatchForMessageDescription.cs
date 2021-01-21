// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Exception that gets thrown when registering a type with a <see cref="MessageDescription"/> for a different type.
    /// </summary>
    public class TypeMismatchForMessageDescription : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMismatchForMessageDescription"/> class.
        /// </summary>
        /// <param name="registeredType"><see cref="Type"/> being registered with.</param>
        /// <param name="messageDescriptionType"><see cref="Type"/> in <see cref="MessageDescription"/>.</param>
        public TypeMismatchForMessageDescription(Type registeredType, Type messageDescriptionType)
            : base($"Type '{registeredType.AssemblyQualifiedName} does not match the type in MessageDescription; '{messageDescriptionType.AssemblyQualifiedName}'")
        {
        }
    }
}