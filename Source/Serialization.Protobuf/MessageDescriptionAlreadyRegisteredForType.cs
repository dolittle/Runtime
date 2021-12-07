// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Serialization.Protobuf;

/// <summary>
/// Exception that gets thrown when a <see cref="Type"/> already has a <see cref="MessageDescription"/> associated with it.
/// </summary>
public class MessageDescriptionAlreadyRegisteredForType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDescriptionAlreadyRegisteredForType"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> trying to register twice for.</param>
    public MessageDescriptionAlreadyRegisteredForType(Type type)
        : base($"Type '{type.AssemblyQualifiedName}' already has a MessageDescription registered")
    {
    }
}