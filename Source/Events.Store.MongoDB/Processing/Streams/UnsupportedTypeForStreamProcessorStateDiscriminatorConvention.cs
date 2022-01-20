// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

/// <summary>
/// Exception that gets thrown when trying to use an unsupported type in  <see cref="StreamProcessorStateDiscriminatorConvention"/>.
/// </summary>
public class UnsupportedTypeForStreamProcessorStateDiscriminatorConvention : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedTypeForStreamProcessorStateDiscriminatorConvention"/> class.
    /// </summary>
    /// <param name="type">Nominal type used in the discriminator convention.</param>
    public UnsupportedTypeForStreamProcessorStateDiscriminatorConvention(Type type)
        : base($"Type: {type} isn't derived from {typeof(AbstractStreamProcessorState)} and is not supported by {typeof(StreamProcessorStateDiscriminatorConvention)}. Was this type erroneously registered with BsonSerializer.RegisterDiscriminatorConvention?", null)
    {
    }
}