// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Dolittle.Runtime.Assemblies
{
    /// <summary>
    /// Represents an implementation of <see cref="IAssemblyUtility"/>.
    /// </summary>
    public class AssemblyUtility : IAssemblyUtility
    {
        /// <inheritdoc/>
        public bool IsAssembly(Library library)
        {
            var path = string.Empty;
            if (library is CompilationLibrary compilationLibrary) path = compilationLibrary.ResolveReferencePaths().FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return true;

            // Borrowed from : http://stackoverflow.com/questions/8593264/determining-if-a-dll-is-a-valid-clr-dll-by-reading-the-pe-directly-64bit-issue
            using var fs = new FileStream(library.Path, FileMode.Open, FileAccess.Read);
            try
            {
                using var reader = new BinaryReader(fs);
                // PE Header starts @ 0x3C (60). Its a 4 byte header.
                fs.Position = 0x3C;

                var peHeader = reader.ReadUInt32();

                // Moving to PE Header start location...
                fs.Position = peHeader;
                var peHeaderSignature = reader.ReadUInt32();
                var machine = reader.ReadUInt16();
                var sections = reader.ReadUInt16();
                var timestamp = reader.ReadUInt32();
                var pSymbolTable = reader.ReadUInt32();
                var noOfSymbol = reader.ReadUInt32();
                var optionalHeaderSize = reader.ReadUInt16();
                var characteristics = reader.ReadUInt16();

                var posEndOfHeader = fs.Position;
                var magic = reader.ReadUInt16();

                var off = 0x60; // Offset to data directories for 32Bit PE images, See section 3.4 of the PE format specification.

                // 0x20b == PE32+ (64Bit), 0x10b == PE32 (32Bit)
                if (magic == 0x20b)
                {
                    // Offset to data directories for 64Bit PE images
                    off = 0x70;
                }

                fs.Position = posEndOfHeader;

                var dataDictionaryRVA = new uint[16];
                var dataDictionarySize = new uint[16];
                fs.Position = Convert.ToUInt16(Convert.ToUInt16(fs.Position) + off);

                for (var i = 0; i < 15; i++)
                {
                    dataDictionaryRVA[i] = reader.ReadUInt32();
                    dataDictionarySize[i] = reader.ReadUInt32();
                }

                return dataDictionaryRVA[14] != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool IsDynamic(Assembly assembly)
        {
            return assembly.IsDynamic;
        }
    }
}
