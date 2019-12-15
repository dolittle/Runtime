// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Validation.MetaData
{
    /// <summary>
    /// Represents the validation meta data for the Regex rule.
    /// </summary>
    public class Regex : Rule
    {
        /// <summary>
        /// Gets or sets the expression that the rule represents.
        /// </summary>
        public string Expression { get; set; }
    }
}
