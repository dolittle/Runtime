// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Runtime.Assemblies;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Scheduling;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Represents well known associations that will be available past the initial system boot stages.
    /// </summary>
    public static class WellKnownAssociations
    {
        /// <summary>
        /// The entry <see cref="Assembly"/> defined.
        /// </summary>
        public const string EntryAssembly = "EntryAssembly";

        /// <summary>
        /// The <see cref="IScheduler">scheduler</see> to be used.
        /// </summary>
        public const string Scheduler = "Scheduler";

        /// <summary>
        /// The <see cref="IAssemblies">assemblies</see> available.
        /// </summary>
        public const string Assemblies = "Assemblies";

        /// <summary>
        /// The <see cref="ITypeFinder"/>.
        /// </summary>
        public const string TypeFinder = "TypeFinder";

        /// <summary>
        /// Which <see cref="Execution.Environment"/> we're running in.
        /// </summary>
        public const string Environment = "Environment";

        /// <summary>
        /// The <see cref="ILoggerManager"/>.
        /// </summary>
        public const string LoggerManager = "LoggerManager";

        /// <summary>
        /// Current <see cref="IBindingCollection">bindings</see>.
        /// </summary>
        public const string Bindings = "Bindings";

        /// <summary>
        /// The <see cref="ICanNotifyForNewBindings">notifications hub</see> for new <see cref="Binding">bindings</see>.
        /// </summary>
        public const string NewBindingsNotificationHub = "NewBindingsNotificationHub";
    }
}