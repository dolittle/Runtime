// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Time;

namespace Dolittle.Runtime.Booting.Stages
{
    /// <summary>
    /// Extensions for building <see cref="InitialSystemSettings"/>.
    /// </summary>
    public static class InitialSystemBootBuilderExtensions
    {
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
