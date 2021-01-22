// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Defines a system for managing <see cref="IRuleContext">rule contexts</see>.
    /// </summary>
    public interface IRuleContexts
    {
        /// <summary>
        /// Get a <see cref="IRuleContext"/> for a given instance.
        /// </summary>
        /// <param name="instance">Instance to get for.</param>
        /// <returns><see cref="IRuleContext"/> to use.</returns>
        IRuleContext GetFor(object instance);
    }
}
