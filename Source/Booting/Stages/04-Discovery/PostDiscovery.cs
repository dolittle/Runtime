// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Booting.Stages
{
    /// <summary>
    /// Represents something that runs post the <see cref="BootStage.Discovery"/> stage of booting.
    /// </summary>
    public class PostDiscovery : ICanRunAfterBootStage<DiscoverySettings>
    {
        readonly Action<ITypeFinder> _callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostDiscovery"/> class.
        /// </summary>
        /// <param name="callback"><see cref="Action{T}">Callback</see> to call with <see cref="ITypeFinder"/>.</param>
        public PostDiscovery(Action<ITypeFinder> callback)
        {
            _callback = callback;
        }

        /// <inheritdoc/>
        public BootStage BootStage => BootStage.Discovery;

        /// <inheritdoc/>
        public void Perform(DiscoverySettings settings, IBootStageBuilder builder)
        {
            var typeFinder = builder.GetAssociation(WellKnownAssociations.TypeFinder) as ITypeFinder;
            _callback(typeFinder);
        }
    }
}