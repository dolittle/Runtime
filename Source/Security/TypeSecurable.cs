// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a <see cref="Securable"/> that applies to a specific <see cref="System.Type"/>.
    /// </summary>
    public class TypeSecurable : Securable
    {
        const string TYPE = "OfType_{{{0}}}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSecurable"/> class.
        /// </summary>
        /// <param name="type"><see cref="System.Type"/> to secure.</param>
        public TypeSecurable(Type type)
            : base(string.Format(CultureInfo.InvariantCulture, TYPE, type.FullName))
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type that is secured.
        /// </summary>
        public Type Type { get; }

        /// <inheritdoc/>
        public override bool CanAuthorize(object actionToAuthorize)
        {
            return actionToAuthorize != null && Type == actionToAuthorize.GetType();
        }
    }
}
