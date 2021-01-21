// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Build
{
    /// <summary>
    /// Defines a system that knows about <see cref="ICanModifyTargetAssembly">modifiers</see>
    /// and is able to work with them on the target assembly for the build.
    /// </summary>
    public interface ITargetAssemblyModifiers
    {
        /// <summary>
        /// Adds a modifier to the chain of modifications to be made.
        /// </summary>
        /// <param name="modifier"><see cref="ICanModifyTargetAssembly">Modifier</see> to add.</param>
        void AddModifier(ICanModifyTargetAssembly modifier);

        /// <summary>
        /// Modify and save the assembly.
        /// </summary>
        void ModifyAndSave();
    }
}