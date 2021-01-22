// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;

namespace Dolittle.Runtime.IO
{
    /// <summary>
    /// Defines functionality for accessing the filesystem.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Get the current directory.
        /// </summary>
        /// <returns>Path to the current directory.</returns>
        string GetCurrentDirectory();

        /// <summary>
        /// Checks wether or not a directory exists.
        /// </summary>
        /// <param name="path">Path to check.</param>
        /// <returns>True if exists, false if not.</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Checks wether or not a path exists.
        /// </summary>
        /// <param name="path">Path to check.</param>
        /// <returns>True if exists, false if not.</returns>
        bool Exists(string path);

        /// <summary>
        /// Read all text from a file.
        /// </summary>
        /// <param name="filename">Path and filename.</param>
        /// <returns>Content of file.</returns>
        string ReadAllText(string filename);

        /// <summary>
        /// Read all text from a file.
        /// </summary>
        /// <param name="filename">Path and filename.</param>
        /// <param name="content">Content of file.</param>
        void WriteAllText(string filename, string content);

        /// <summary>
        /// Get files for a specific path.
        /// </summary>
        /// <param name="path">Path to get files from.</param>
        /// <param name="searchPattern">Search pattern to use for filtering.</param>
        /// <returns><see cref="IEnumerable{FileInfo}">Enumerable of <see cref="FileInfo"/></see>.</returns>
        IEnumerable<FileInfo> GetFilesFrom(string path, string searchPattern);

        /// <summary>
        /// Get directories at a specific path.
        /// </summary>
        /// <param name="path">Path to get from.</param>
        /// <returns><see cref="IEnumerable{T}"/> of paths.</returns>
        IEnumerable<string> GetDirectoriesIn(string path);
    }
}
