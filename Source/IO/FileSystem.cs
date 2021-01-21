// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;

namespace Dolittle.Runtime.IO
{
    /// <summary>
    /// Represents an implementation of <see cref="IFileSystem"/>.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        /// <inheritdoc/>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc/>
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc/>
        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        /// <inheritdoc/>
        public IEnumerable<FileInfo> GetFilesFrom(string path, string searchPattern)
        {
            return new DirectoryInfo(path).GetFiles(searchPattern);
        }

        /// <inheritdoc/>
        public string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }

        /// <inheritdoc/>
        public void WriteAllText(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetDirectoriesIn(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
