// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using Dolittle.Runtime.Collections;

namespace Dolittle.Runtime.Configuration.Files
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanProvideConfigurationObjects"/> for files on disk.
    /// </summary>
    /// <remarks>
    /// This provider looks for configuration files in a specific <see cref="BaseFolder">folder</see>
    /// relative to current running directory. It also assumes a specific convention for filenames
    /// with the name of the <see cref="IConfigurationObject"/>. The name will be the same as provided
    /// in the <see cref="NameAttribute"/>, or fall back to the type name.
    /// </remarks>
    public class FileConfigurationObjectsProvider : ICanProvideConfigurationObjects
    {
        /// <summary>
        /// The base folder for Dolittle configuration files.
        /// </summary>
        public const string BaseFolder = ".dolittle";

        readonly IConfigurationFileParsers _parsers;

        readonly string[] _searchPaths = new[]
        {
            BaseFolder,
            "config",
            "/config",
            "data",
            "..",
            "."
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurationObjectsProvider"/> class.
        /// </summary>
        /// <param name="parsers"><see cref="IConfigurationFileParsers"/> for parsing.</param>
        public FileConfigurationObjectsProvider(IConfigurationFileParsers parsers)
        {
            _parsers = parsers;
        }

        /// <inheritdoc/>
        public bool CanProvide(Type type)
        {
            var foundPaths = new List<string>();
            _searchPaths.ForEach(_ =>
            {
                var filename = GetFilenameFor(type, _);
                if (File.Exists(filename)) foundPaths.Add(filename);
            });
            if (foundPaths.Count > 1) throw new MultipleFilesAvailableOfSameType(type, foundPaths);
            return foundPaths.Count > 0;
        }

        /// <inheritdoc/>
        public object Provide(Type type)
        {
            object instance = null;
            _searchPaths.ForEach(_ =>
            {
                var filename = GetFilenameFor(type, _);
                if (File.Exists(filename))
                {
                    var content = File.ReadAllText(filename);
                    instance = _parsers.Parse(type, filename, content);
                }
            });

            if (instance != null) return instance;
            throw new UnableToProvideConfigurationObject<FileConfigurationObjectsProvider>(type);
        }

        string GetFilenameFor(Type type, string basePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), basePath, $"{type.GetFriendlyConfigurationName()}.json");
        }
    }
}
