// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Behaviors
{
    /// <summary>
    /// The callback for a trigger when it gets triggered.
    /// </summary>
    /// <param name="context"><see cref="TriggerContext"/>.</param>
    public delegate void Triggered(TriggerContext context);
}