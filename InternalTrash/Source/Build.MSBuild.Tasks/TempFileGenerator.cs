// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Dolittle.Build.MSBuild.Tasks
{
    /// <summary>
    /// Represents a task that generates a temporary file in the %tmp% folder.
    /// </summary>
    public class TempFileGenerator : Task
    {
        /// <summary>
        /// Gets or sets the filename generated.
        /// </summary>
        [Output]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the filename for the portable debug info (PDB) file.
        /// </summary>
        [Output]
        public string DebugInfoFileName { get; set; }

        /// <inheritdoc/>
        public override bool Execute()
        {
            var tempFileName = Path.Combine(Path.GetTempPath(), $"tmp_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}");
            FileName = $"{tempFileName}.dll";
            DebugInfoFileName = $"{tempFileName}.pdb";

            return true;
        }
    }
}