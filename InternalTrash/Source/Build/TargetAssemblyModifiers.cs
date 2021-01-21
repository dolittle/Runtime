// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using Dolittle.Runtime.Lifecycle;
using Mono.Cecil;

namespace Dolittle.Build
{
    /// <summary>
    /// Represents an implementation of <see cref="ITargetAssemblyModifiers"/>.
    /// </summary>
    [Singleton]
    public class TargetAssemblyModifiers : ITargetAssemblyModifiers
    {
        readonly List<ICanModifyTargetAssembly> _modifiers = new List<ICanModifyTargetAssembly>();
        readonly BuildTarget _configuration;
        readonly IBuildMessages _buildMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetAssemblyModifiers"/> class.
        /// </summary>
        /// <param name="configuration"><see cref="BuildTarget"/> to use.</param>
        /// <param name="buildMessages"><see cref="IBuildMessages"/> for build messages.</param>
        public TargetAssemblyModifiers(
            BuildTarget configuration,
            IBuildMessages buildMessages)
        {
            _configuration = configuration;
            _buildMessages = buildMessages;
        }

        /// <inheritdoc/>
        public void AddModifier(ICanModifyTargetAssembly modifier)
        {
            _modifiers.Add(modifier);
        }

        /// <inheritdoc/>
        public void ModifyAndSave()
        {
            if (_modifiers.Count == 0)
            {
                File.Copy(_configuration.TargetAssemblyPath, _configuration.OutputAssemblyPath);
                return;
            }

            _buildMessages.Information("Performing assembly modifications");
            _buildMessages.Indent();

            var debugInfoPath = Path.Combine(
                                    Path.GetDirectoryName(_configuration.TargetAssemblyPath),
                                    $"{Path.GetFileNameWithoutExtension(_configuration.TargetAssemblyPath)}.pdb");

            var readerParameters = new ReaderParameters(ReadingMode.Immediate);
            if (File.Exists(debugInfoPath))
            {
                _buildMessages.Information($"Including debug information for the target assembly from '{debugInfoPath}'");
                readerParameters.ReadSymbols = true;
            }

            using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(_configuration.TargetAssemblyPath, readerParameters))
            {
                _modifiers.ForEach(_ =>
                {
                    _buildMessages.Information($"{_.Message} (Modifier: '{_.GetType().AssemblyQualifiedName}')");
                    _buildMessages.Indent();
                    _.Modify(assemblyDefinition);
                    _buildMessages.Unindent();
                });
                _buildMessages.Information($"Write modified assembly to '{_configuration.OutputAssemblyPath}'");

                assemblyDefinition.Write(
                    _configuration.OutputAssemblyPath,
                    new WriterParameters
                    {
                        WriteSymbols = true
                    });
            }

            _buildMessages.Unindent();
        }
    }
}