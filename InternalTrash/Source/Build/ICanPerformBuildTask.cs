// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Build
{
    /// <summary>
    /// Defines an interface for systems that will perform post build tasks.
    /// </summary>
    public interface ICanPerformBuildTask
    {
        /// <summary>
        /// Gets the message string to show in output.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Method that gets called for performing the necessary build tasks.
        /// </summary>
        void Perform();
    }
}