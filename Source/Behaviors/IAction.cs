// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Behaviors
{
    /// <summary>
    /// Defines the basics of an action that gets performed as the consequence of a <see cref="ITrigger"/> or
    /// part of a <see cref="IBehavior"/>.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Performs the action.
        /// </summary>
        void Perform();
    }
}