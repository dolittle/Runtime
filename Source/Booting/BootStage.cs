// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Defines the different stages of booting.
    /// </summary>
    public enum BootStage
    {
        /// <summary>
        /// Basics stage - fixed.
        /// </summary>
        /// <remarks>
        /// This stage is defined by the system and can't be swapped out. It also does not support
        /// any <see cref="ICanRunBeforeBootStage{T}"/> or <see cref="ICanRunAfterBootStage{T}"/>.
        /// </remarks>
        Basics = 1,

        /// <summary>
        /// Logging stage - fixed.
        /// </summary>
        /// <remarks>
        /// This stage is defined by the system and can't be swapped out. It also does not support
        /// any <see cref="ICanRunBeforeBootStage{T}"/> or <see cref="ICanRunAfterBootStage{T}"/>.
        /// </remarks>
        Logging,

        /// <summary>
        /// Discovery stage - fixed.
        /// </summary>
        /// <remarks>
        /// This stage is defined by the system and can't be swapped out. It also does not support
        /// any <see cref="ICanRunBeforeBootStage{T}"/> or <see cref="ICanRunAfterBootStage{T}"/>.
        /// </remarks>
        Discovery = 4,

        /// <summary>
        /// Prepare boot - after this stage there should be a <see cref="IContainer"/> available -
        /// most likely a temporary container used during booting.
        /// </summary>
        PrepareBoot,

        /// <summary>
        /// Configuration is hooked up. After this, all systems can start relying on configuration to be there.
        /// </summary>
        Configuration,

        /// <summary>
        /// Main <see cref="IContainer"/> hookup.
        /// </summary>
        Container,

        /// <summary>
        /// Main running of <see cref="ICanPerformBootProcedure">boot procedures</see>.
        /// </summary>
        BootProcedures
    }
}
