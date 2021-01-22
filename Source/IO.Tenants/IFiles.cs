// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.IO.Tenants
{
    /// <summary>
    /// Defines a file system that will provide a sandbox specific to the current tenant in the <see cref="ExecutionContext"/>.
    /// </summary>
    public interface IFiles
    {
        /// <summary>
        /// Get directories at a specific path.
        /// </summary>
        /// <param name="relativePath">Relative path to get from.</param>
        /// <returns><see cref="IEnumerable{T}"/> of paths.</returns>
        IEnumerable<string> GetDirectoriesIn(string relativePath);

       /// <summary>
        /// Check if a directory exists based on the relative path within the tenants sandbox.
        /// </summary>
        /// <param name="relativePath">Relative path to check.</param>
        /// <returns>True if exists, false if not.</returns>
        bool DirectoryExists(string relativePath);

        /// <summary>
        /// Check if a file exists based on the relative path within the tenants sandbox.
        /// </summary>
        /// <param name="relativePath">Relative path to check.</param>
        /// <returns>True if exists, false if not.</returns>
        bool Exists(string relativePath);

        /// <summary>
        /// Read all text from a relative path within the tenant.
        /// </summary>
        /// <param name="relativePath">Relative path to the file to read from.</param>
        /// <returns>Content of the file.</returns>
        string ReadAllText(string relativePath);

        /// <summary>
        /// Write all text from a relative path within the tenant.
        /// </summary>
        /// <param name="relativePath">Relative path to the file to read from.</param>
        /// <param name="content">Content of the file.</param>
        void WriteAllText(string relativePath, string content);
    }
}