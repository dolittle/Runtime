// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Validation.MetaData
{
    /// <summary>
    /// Represents the validation metadata for a type.
    /// </summary>
    public class TypeMetaData
    {
        /// <summary>
        /// Gets the properties with rulesets.
        /// </summary>
        public Dictionary<string, Dictionary<string, Rule>> Properties { get; } = new Dictionary<string, Dictionary<string, Rule>>();

        /// <summary>
        /// Gets the ruleset for a specific property.
        /// </summary>
        /// <param name="property">Name of property.</param>
        /// <returns>Ruleset in the form of a <see cref="Dictionary{TKey, TValue}"/>.</returns>
        public Dictionary<string, Rule> this[string property]
        {
            get
            {
                if (!Properties.ContainsKey(property))
                    Properties[property] = new Dictionary<string, Rule>();
                return Properties[property];
            }
        }
    }
}
