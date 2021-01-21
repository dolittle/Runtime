// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Defines a system that is capable of notifying when there are new <see cref="Binding">bindings</see>
    /// available during booting.
    /// </summary>
    public interface ICanNotifyForNewBindings
    {
        /// <summary>
        /// Notify any subscribers with new bindings.
        /// </summary>
        /// <param name="bindings">New <see cref="IBindingCollection">bindings</see>.</param>
        void Notify(IBindingCollection bindings);

        /// <summary>
        /// Subscribe to new <see cref="IBindingCollection">bindings</see> that will occur during booting.
        /// </summary>
        /// <param name="subscriber"><see cref="Action"/> that gets called with the new <see cref="IBindingCollection">bindings</see>.</param>
        void SubscribeTo(Action<IBindingCollection> subscriber);
    }
}
