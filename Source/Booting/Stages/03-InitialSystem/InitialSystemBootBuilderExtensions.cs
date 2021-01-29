// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Booting.Stages;
using Dolittle.Runtime.Scheduling;
using Dolittle.Runtime.Time;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Extensions for building <see cref="InitialSystemSettings"/>.
    /// </summary>
    public static class InitialSystemBootBuilderExtensions
    {
        /// <summary>
        /// Set scheduling to be synchronous.
        /// </summary>
        /// <param name="bootBuilder"><see cref="BootBuilder"/> to build.</param>
        /// <returns>Chained <see cref="BootBuilder"/>.</returns>
        /// <remarks>
        /// Asynchronous scheduling is default.
        /// </remarks>
        public static IBootBuilder SynchronousScheduling(this IBootBuilder bootBuilder)
        {
            bootBuilder.Set<InitialSystemSettings>(_ => _.Scheduler, new SyncScheduler());
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
