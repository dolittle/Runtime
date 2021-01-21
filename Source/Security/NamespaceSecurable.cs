// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a <see cref="Securable"/> that applies to a specific namespace.
    /// </summary>
    public class NamespaceSecurable : Securable
    {
        const string NAMESPACE = "InNamespace_{{{0}}}";

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceSecurable"/> class.
        /// </summary>
        /// <param name="namespace">Namespace to secure.</param>
        public NamespaceSecurable(string @namespace)
            : base(string.Format(CultureInfo.InvariantCulture, NAMESPACE, @namespace))
        {
            Namespace = @namespace;
        }

        /// <summary>
        /// Gets the namespace that is secured.
        /// </summary>
        public string Namespace { get; }

        /// <inheritdoc/>
        public override bool CanAuthorize(object actionToAuthorize)
        {
            return actionToAuthorize?.GetType().Namespace.StartsWith(
                Namespace,
                StringComparison.Ordinal) == true;
        }
    }
}
