// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7.Models.Events
{
    /// <summary>
    /// Represents a claim.
    /// </summary>
    public class Claim
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Claim"/> class.
        /// </summary>
        /// <param name="name">The name of the claim.</param>
        /// <param name="value">The value of the claim.</param>
        /// <param name="valueType">The type of the value of the claim.</param>
        public Claim(string name, string value, string valueType)
        {
            Name = name;
            Value = value;
            ValueType = valueType;
        }

        /// <summary>
        /// Gets or sets the name of the claim.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the claim.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the value of the claim.
        /// </summary>
        public string ValueType { get; set; }
    }
}
