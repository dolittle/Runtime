/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.DependencyInversion.Conventions;

namespace doLittle.Applications
{
    /// <summary>
    /// Represents an implementation of <see cref="IApplication"/>
    /// </summary>
    [IgnoreDefaultConvention]
    public class Application : IApplication
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Application"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="structure"></param>
        public Application(ApplicationName name, IApplicationStructure structure)
        {
            Name = name;
            Structure = structure;
        }

        /// <inheritdoc/>
        public ApplicationName Name { get; }

        /// <inheritdoc/>
        public IApplicationStructure Structure { get; }
    }
}
