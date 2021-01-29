// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting.Stages;
using Dolittle.Runtime.IO;
using Dolittle.Runtime.Time;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Extensions for building <see cref="InitialSystemSettings"/>.
    /// </summary>
    public static class InitialSystemBootBuilderExtensions
    {
        /// <summary>
        /// Use a specific <see cref="IFileSystem"/>.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <param name="fileSystem"><see cref="IFileSystem"/> to use.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        public static IBootBuilder UseFileSystem(this IBootBuilder bootBuilder, IFileSystem fileSystem)
        {
            bootBuilder.Set<InitialSystemSettings>(_ => _.FileSystem, fileSystem);
            return bootBuilder;
        }

        /// <summary>
        /// Use a specific <see cref="ISystemClock"/>.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <param name="systemClock"><see cref="ISystemClock"/> to use.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        public static IBootBuilder UseSystemClock(this IBootBuilder bootBuilder, ISystemClock systemClock)
        {
            bootBuilder.Set<InitialSystemSettings>(_ => _.SystemClock, systemClock);
            return bootBuilder;
        }
    }
}
