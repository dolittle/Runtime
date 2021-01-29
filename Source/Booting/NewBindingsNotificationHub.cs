// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanNotifyForNewBindings"/>.
    /// </summary>
    public class NewBindingsNotificationHub : ICanNotifyForNewBindings
    {
        event Action<IBindingCollection> _subscribers = (_) => { };

        /// <inheritdoc/>
        public void Notify(IBindingCollection bindings) => _subscribers(bindings);

        /// <inheritdoc/>
        public void SubscribeTo(Action<IBindingCollection> subscriber) => _subscribers += subscriber;
    }
}
