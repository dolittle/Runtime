/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines the configuration manager for <see cref="EventHorizonsConfiguration"/>
    /// </summary>
    public interface IEventHorizonsConfigurationManager
    {
        /// <summary>
        /// Gets the current <see cref="EventHorizonsConfiguration"/>
        /// </summary>
        EventHorizonsConfiguration Current { get; }
    }
}
