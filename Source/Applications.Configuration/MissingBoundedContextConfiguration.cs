/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Exception that gets thrown when there is no configuration for the bounded context
    /// </summary>
    public class MissingBoundedContextConfiguration : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MissingBoundedContextConfiguration"/>
        /// </summary>
        public MissingBoundedContextConfiguration() : base("Missing configuration for the bounded context - looking for file 'bounded-context.json'") {}
        /// <summary>
        /// Initializes a new instance of <see cref="MissingBoundedContextConfiguration"/> with path
        /// </summary>
        /// <param name="path"></param>
        public MissingBoundedContextConfiguration(string path) : base($"Missing configuration for the bounded context - looking for file 'bounded-context.json' at path {path}") {}
    }
}