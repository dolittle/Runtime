// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Validation.MetaData
{
    /// <summary>
    /// Represents the base class of a rule.
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// Gets or sets the message that will be used when rule is not valid.
        /// </summary>
        public string Message { get; set; }
    }
}
