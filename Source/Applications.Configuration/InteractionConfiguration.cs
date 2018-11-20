/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Represents the configuration for an interaction layer of the <see cref="BoundedContext"/>
    /// </summary>
    public class InteractionLayerConfiguration
    {
        /// <summary>
        /// The type of the interaction layer
        /// </summary>
        public string Type {get; set;}
        /// <summary>
        /// The programming language of the interaction layer
        /// </summary>
        public string Language {get; set;}
        /// <summary>
        /// The entrypoint of the interaction layer
        /// </summary>
        public string EntryPoint {get; set;}
        /// <summary>
        /// THe framework of the interaction layer
        /// </summary>
        public string Framework {get; set;}
    }
}