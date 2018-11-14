/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Represents the configuration for the <see cref="BoundedContext"/> core
    /// </summary>
    public class CoreConfiguration
    {
        /// <summary>
        /// The core programming language used in the
        /// </summary>
        public string Language {get; set;}
        /// <summary>
        /// The entrypoint of the <see cref="BoundedContext"/>
        /// </summary>
        public string EntryPoint {get; set;}
    }
}