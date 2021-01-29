// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Scheduling;
using Dolittle.Runtime.Time;

namespace Dolittle.Runtime.Booting.Stages
{
    /// <summary>
    /// Represents the <see cref="BootStage.InitialSystem"/> stage of booting.
    /// </summary>
    public class InitialSystem : ICanPerformBootStage<InitialSystemSettings>
    {
        /// <inheritdoc/>
        public BootStage BootStage => BootStage.InitialSystem;

        /// <inheritdoc/>
        public void Perform(InitialSystemSettings settings, IBootStageBuilder builder)
        {
            var scheduler = settings.Scheduler ?? new AsyncScheduler();

            builder.Associate(WellKnownAssociations.Scheduler, scheduler);

            builder.Bindings.Bind<ISystemClock>().To(settings.SystemClock ?? new SystemClock());
            builder.Bindings.Bind<IScheduler>().To(scheduler);
        }
    }
}
