// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Defines the strategy in which a binding will be provided.
    /// </summary>
    public interface IActivationStrategy
    {
        /// <summary>
        /// Get the target type.
        /// </summary>
        /// <returns>The type of the target - typically the implementing type.</returns>
        Type GetTargetType();
    }
}