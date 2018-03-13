/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Validation.MetaData
{
    /// <summary>
    /// Represents the base class of a rule
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// Gets or sets the message that will be used when rule is not valid
        /// </summary>
        public string Message { get; set; }
    }
}
