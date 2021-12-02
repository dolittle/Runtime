// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Dolittle.Runtime.Serialization.Json;
using Microsoft.Extensions.FileProviders;
using IContentSerializer = Dolittle.Runtime.Serialization.Json.ISerializer;

namespace Dolittle.Runtime.CLI.Configuration.Files;

/// <summary>
/// Represents an implementation of <see cref="ISerializer"/>.
/// </summary>
public class Serializer : ISerializer
{
    readonly IContentSerializer _contentSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Serializer"/> class.
    /// </summary>
    /// <param name="contentSerializer">The serializer to use to serialize the contents of the file.</param>
    public Serializer(IContentSerializer contentSerializer)
    {
        _contentSerializer = contentSerializer;
    }

    /// <inheritdoc />
    public T FromJsonFile<T>(IFileInfo file, ISerializationOptions options = null)
    {
        return (T) FromJsonFile(typeof(T), file, options);
    }

    /// <inheritdoc />
    public object FromJsonFile(Type type, IFileInfo file, ISerializationOptions options = null)
    {
        using var reader = new StreamReader(file.CreateReadStream());
        var contents = reader.ReadToEnd();
        return _contentSerializer.FromJson(type, contents, options);
    }
}