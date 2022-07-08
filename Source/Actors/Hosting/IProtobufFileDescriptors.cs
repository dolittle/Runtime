// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Google.Protobuf.Reflection;

namespace Dolittle.Runtime.Actors.Hosting;

/// <summary>
/// Defines a system that knows about all proto file descriptors used in grain clients.
/// </summary>
public interface IProtobufFileDescriptors
{
    /// <summary>
    /// Gets all discovered <see cref="FileDescriptor"/>.
    /// </summary>
    public IEnumerable<FileDescriptor> All { get; }
}
