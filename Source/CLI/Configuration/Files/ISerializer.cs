// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.FileProviders;

namespace Dolittle.Runtime.CLI.Configuration.Files;

/// <summary>
/// Defines a serializer that operates on files.
/// </summary>
public interface ISerializer
{
    /// <summary>
    /// Deserialize Json to a specific type from the contents of a file.
    /// </summary>
    /// <typeparam name="T">Type to deserialize to.</typeparam>
    /// <param name="file">The <see cref="IFileInfo"/> describing the file to deserialize.</param>
    /// <param name="options">Options for the serializer.</param>.
    /// <returns>A deserialized instance of the specified type.</returns>
    T FromJsonFile<T>(IFileInfo file);
        
    /// <summary>
    /// Deserialize Json to a specific type from the contents of a file.
    /// </summary>
    /// <param name="type">Type to deserialize to.</param>
    /// <param name="file">The <see cref="IFileInfo"/> describing the file to deserialize.</param>
    /// <param name="options">Options for the serializer.</param>.
    /// <returns>A deserialized instance.</returns>
    object FromJsonFile(Type type, IFileInfo file);
}
