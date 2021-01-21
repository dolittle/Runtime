// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Mono.Cecil;

namespace Dolittle.Build
{
    /// <summary>
    /// Represents a <see cref="ICanModifyTargetAssembly"/> that is capable of embedding resources
    /// into an <see cref="AssemblyDefinition">assembly</see>.
    /// </summary>
    public class EmbedResource : ICanModifyTargetAssembly
    {
        readonly string _name;
        readonly byte[] _bytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedResource"/> class.
        /// </summary>
        /// <param name="name">Name of the resource to embed - fully qualified.</param>
        /// <param name="bytes">Byte array to embed.</param>
        public EmbedResource(string name, byte[] bytes)
        {
            _name = name;
            _bytes = bytes;
        }

        /// <inheritdoc/>
        public string Message => $"Embedding resource {_name}";

        /// <inheritdoc/>
        public void Modify(AssemblyDefinition assemblyDefinition)
        {
            var embeddedResource = new EmbeddedResource(_name, ManifestResourceAttributes.Public, _bytes);
            assemblyDefinition.MainModule.Resources.Add(embeddedResource);
        }
    }
}