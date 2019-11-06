/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Defines a system that represents the metrics system
    /// </summary>
    public interface IMetricsSystem
    {
        /// <summary>
        /// Start the system
        /// </summary>
        void Start();
    }
}