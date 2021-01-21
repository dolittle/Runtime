// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.Assemblies;

namespace Dolittle.Build
{
    /// <summary>
    /// Represents the configuration for the build.
    /// </summary>
    public class BuildTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildTarget"/> class.
        /// </summary>
        /// <param name="targetAssemblyPath">Path of the target assembly being built.</param>
        /// <param name="outputAssemblyPath">Path for the output assembly.</param>
        /// <param name="assembly"><see cref="Assembly"/> being built.</param>
        /// <param name="assemblyContext"><see cref="AssemblyContext"/> for the <see cref="Assembly"/> being built.</param>
        public BuildTarget(
            string targetAssemblyPath,
            string outputAssemblyPath,
            Assembly assembly,
            IAssemblyContext assemblyContext)
        {
            TargetAssemblyPath = targetAssemblyPath;
            OutputAssemblyPath = outputAssemblyPath;
            Assembly = assembly;
            AssemblyContext = assemblyContext;
            AssemblyName = assembly.GetName();
        }

        /// <summary>
        /// Gets the path of the target assembly being build.
        /// </summary>
        public string TargetAssemblyPath { get; }

        /// <summary>
        /// Gets the path that represents the output assembly path.
        /// </summary>
        public string OutputAssemblyPath { get; }

        /// <summary>
        /// Gets the <see cref="Assembly"/> being built.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyContext"/> for the <see cref="Assembly"/> being built.
        /// </summary>
        public IAssemblyContext AssemblyContext { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyName"/> for the <see cref="Assembly"/>.
        /// </summary>
        public AssemblyName AssemblyName { get; }
    }
}